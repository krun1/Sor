using BasicComponents.Monad;
using Sor.API;
using Sor.Filter;
using Sor.Wrappers;

namespace Sor;



internal sealed class FilterVisitor<T>(IInputNode? input, ExpressionVisitor<T> parent)
{
    public IInputNode? Visit(FilterGrammarParser.ContextualOperationContext contextualOperation)
    {
        return new InjectionFunction<T>(contextualOperation.operation(), parent);
    }

    public IInputNode? Visit(FilterGrammarParser.OperationContext operation)
    {
        if (operation.accessor() is {} accessor)
            return Visit(accessor);
        var leftOperation = operation.operation()[0];
        var rightOperation = operation.operation()[1];
        var left = Visit(leftOperation);
        var right = Visit(rightOperation);
        
        if (operation.EQ() != null)
            return left.IsEqual(right);
        if (operation.DIFF() != null)
            return left.IsNotEqual(right);
        var leftValue = (IInputValue?)left;
        var rightValue = (IInputValue?)right;
        
        if (operation.PLUS() != null)
            return leftValue.Plus(rightValue);
        if (operation.MINUS() != null)
            return leftValue.Minus(rightValue);
        if (operation.TIMES() != null)
            return leftValue.Multiply(rightValue);
        if (operation.DIV() != null)
            return leftValue.Div(rightValue);
        if (operation.GT() != null)
            return leftValue.GreaterThan(rightValue);
        if (operation.LT() != null)
            return leftValue.LessThan(rightValue);
        if (operation.GTE() != null)
            return leftValue.GreaterOrEqualThan(rightValue);
        if (operation.LTE() != null)
            return leftValue.LessOrEqualThan(rightValue);
        throw new InvalidOperationException(operation.GetText());
    }
    
    public IInputNode? Visit(FilterGrammarParser.AccessorContext operation)
    {
        if (operation.fieldAccessor() is {} accessor)
            return VisitAccessor(accessor);
        if (operation.variable() is {} variable)
            return VisitVariable(variable);
        if (operation.function() is {} function)
            return VisitFunction(function);
        if (operation.@const() is { } @const)
            return VisitConst(@const);
        return input;
    }

    private IInputNode? VisitConst(FilterGrammarParser.ConstContext @const)
    {
        if (@const.INT() is {} integer)
            return InputHelper.Wrap(integer.GetInt());
        if (@const.STRING() is {} s)
            return InputHelper.Wrap(s.GetString());
        throw new InvalidOperationException(@const.GetText());
    }

    private IInputNode? VisitAccessor(FilterGrammarParser.FieldAccessorContext context)
    {
        if (context.fieldName() != null)
            return VisitAccessor(context.fieldName(), context.nextAccessor(), input);
        if (context.array() != null)
            return VisitAccessor(context.array(), context.nextAccessor(), input);
        throw new NotImplementedException();
    }

    private IInputNode? VisitVariable(FilterGrammarParser.VariableContext variable)
    {
        var v = parent.Variables.GetValueOrDefault(variable.ID().GetText());
        
        if (variable.nextAccessor() is {} accessor)
        {
            if (accessor.fieldName() is {} fieldName)
                return VisitAccessor(fieldName, accessor.nextAccessor(), v);
            if (accessor.array() is {} array)
                return VisitAccessor(array, accessor.nextAccessor(), v);
        }
        return v;
    }

    private IInputNode? VisitFunction(FilterGrammarParser.FunctionContext function)
    {
        var parameters = function.parameter()
            .Select(VisitParameter)
            .Select(node => node?.WithContext(InputContext.Parameter))
            .ToList();

        return parent.Functions.Invoke(function.ID().GetText(), parameters);
    }

    private IInputNode? VisitParameter(FilterGrammarParser.ParameterContext parameter)
    {
        if (parameter.operation() is {} operation)
            return Visit(operation);
        else
            return Visit(parameter.contextualOperation());
    }    
    
    private IInputNode? VisitAccessor(
        Either<FilterGrammarParser.FieldNameContext, FilterGrammarParser.ArrayContext> context,
        FilterGrammarParser.NextAccessorContext? nextAccessor,
        IInputNode? inputNode)
    {
        var result = context.Resolve(
            nameContext => VisitField(nameContext, inputNode),
            arrayContext => VisitArray(arrayContext, inputNode));

        if (nextAccessor != null)
        {
            if (nextAccessor.fieldName() != null)
                result = VisitAccessor(nextAccessor.fieldName(), nextAccessor.nextAccessor(), result);
            if (nextAccessor.array() != null)
                result = VisitAccessor(nextAccessor.array(), nextAccessor.nextAccessor(), result);
        }
        return result?.WithContext(InputContext.Parsing);
    }

    private IInputNode? VisitField(FilterGrammarParser.FieldNameContext nameContext, IInputNode? inputNode)
    {
        return (inputNode as IInputObject)?.GetField(nameContext.GetText());
    }

    private IInputNode? VisitArray(FilterGrammarParser.ArrayContext arrayContext, IInputNode? inputNode)
    {
        if (inputNode is not IInputArray array)
            return null;
        if (arrayContext.INT() is {} index)
            return array.GetAt(index.GetInt());
        if (arrayContext.range() is {} range)
            return new RangeInputArray(array.GetRange( new Range(range.INT()[0].GetInt(), range.INT()[1].GetInt())));
        if (arrayContext.takeRange() is {} take)
            return new RangeInputArray(array.GetRange(Range.EndAt(take.INT().GetInt())));
        if (arrayContext.skipRange() is {} skip)
            return new RangeInputArray(array.GetRange(Range.StartAt(skip.INT().GetInt())));
        return new RangeInputArray(array.GetRange(Range.All));
    }
}