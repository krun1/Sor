grammar FilterGrammar;



/*
 * Parser Rules
 */
expression         : (readerDef content)+ writerDef EOF ;

/*
File parsing :

from file C:/somePath

$content

TO ResultType
*/

readerDef           : FROM readerId url;
readerId            : ID;
url                 : STRING;

writerDef           : TO writerId;
writerId            : ID;

/*
Content Parsing :

.FieldName
.[]
.[5]
*/

content             : (variable ASSIGNMENT_OPERATOR)? operation (CONTENT_SEPARATOR content)?;
operation           : operation (TIMES | DIV) operation
                    | operation (PLUS | MINUS) operation
                    | operation (EQ | DIFF) operation
                    | operation (GTE | LTE | GT | LT) operation
                    | accessor ;

accessor            : DOT | (DOT? fieldAccessor) | variable | function | const;
function            : ID OPENPARAM (parameter (',' parameter)*)? CLOSEPARAM;
parameter           : operation | contextualOperation ;
fieldAccessor       : (fieldName | array) nextAccessor?;
nextAccessor        : (DOT fieldName | array) nextAccessor?;
variable            : VARIABLE_KEYWORD ID nextAccessor?;
const               : INT | STRING ;
fieldName           : ID;
array               : OPENARRAY (INT | range | skipRange | takeRange)? CLOSEARRAY;
contextualOperation: CONTEXTUAL_OPERATOR operation ;

skipRange           : INT DOUBLEDOT;
takeRange           : DOUBLEDOT INT;
range               : INT DOUBLEDOT INT;

/*
 * Lexer Rules
 */

fragment F          : 'f' | 'F';
fragment R          : 'r' | 'R';
fragment O          : 'o' | 'O';
fragment M          : 'm' | 'M';
fragment T          : 't' | 'T';

FROM                : F R O M ;
TO                  : T O ;
fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment NUMBER     : [0-9] ;
fragment ESC        : '\\"' ;
DOUBLEDOT           : '..'  ;
VARIABLE_KEYWORD    : '$' ;

PLUS                : '+' ;
MINUS               : '-' ;
TIMES               : '*' ;
DIV                 : '/' ;
GTE                 : '>=' ;
LTE                 : '<=' ;
GT                  : '>' ;
LT                  : '<' ;
EQ                  : '==' ;
DIFF                : '!=' ;

ASSIGNMENT_OPERATOR : '=' ;
CONTEXTUAL_OPERATOR : ':' ;
CONTENT_SEPARATOR   : ';' ;

STRING              : '"' (ESC | ~["] )* '"';
DOT                 : '.';
INT                 : (MINUS)? NUMBER ;
ID                  : (LOWERCASE | UPPERCASE | NUMBER)+ ;
OPENARRAY           : '[' ;
CLOSEARRAY          : ']' ;
OPENPARAM           : '(' ;
CLOSEPARAM          : ')' ;

WS                  : (' ' | '\t' | '\r' | '\n') -> skip;

