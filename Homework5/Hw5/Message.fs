namespace Hw5

type Message =
    | SuccessfulExecution = 0
    | WrongArgLength = 1
    | WrongArg1Format = 2
    | WrongArg2Format = 3
    | WrongArgFormatOperation = 4
    | DivideByZero = 5