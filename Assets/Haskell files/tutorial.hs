-- It all begins... with types and expressions....

-- Types, as you can guess, are how Haskell describes the kind of data you're working with.
-- There are a few standard types in Haskell:
-- 1) Int: an integer within the range -2^29 to (2^29 - 1)
-- 2) Integer: an unbounded integer
-- 3) Float: a floating-point number
-- 4) Double: a double-precision floating-point number
-- 5) Char: a single character (e.g. 'a', '1', '£')
-- 6) Bool: a boolean value, either True or False

-- Expressions, meanwhile, are sections of code that can be evaluated to produce a value.
-- It's similar to the mathematical definition: 2 + 2 is an expression that evaluates to 4.
-- In Haskell, expressions can be as simple as a single value or as complex as a function call with multiple arguments.

-- Types and expressions on their own aren't of much use, however, as Haskell doesn't have conventional 'variables'.
-- This brings us to the building block of Haskell: functions.

-- Functions are similar to the mathematical definition: they take one or more inputs and return an output value.
-- Within Haskell, functions need to follow a very specific format. For example:

incrementInt :: Int -> Int -- this is the function's type signature. Int -> Int is the type of the function: takes an Int as input and returns an Int output
incrementInt x = x + 1 -- this is the function declaration: x is the input parameter and x + 1 is the expression (body) that defines the output

-- A function can also have multiple inputs in Haskell:

addInts :: Int -> Int -> Int -- here, since the last type has to be the output, the first two are correctly parsed as the required input types
addInts x y = x + y

-- A brief note here: "Int -> Int" and "Int -> Int -> Int" are, in themselves, unique types.
-- The implications of this will be explored later.

-- Now, for your journey. There are going to be obstacles in your path. 
-- Monsters and enemies that have perverted the power of Haskell for their own gain.
-- They're going to try to attack you, and you're going to have to learn to defend yourself.

-- You've been granted the power of the Lambda Gauntlet.
-- You can examine the code within your opponents and, most importantly, you can *change it*.

-- Use the gauntlet now, to make sure you don't take any damage from my attack!

type Health = Int -- this allows you to define custom types as aliases for other types

data Player = Player Health -- this defines a custom data structure with a specific pattern

hurtPlayer :: Player -> Player
hurtPlayer (Player health) = Player (health - 10) 

-- The "(Player health)" there is an example of pattern-matching.
-- You can specify patterns that data should match to, and then deconstructing that data if it does, in one fell swoop.

-- Your job: change that function there so you take no damage at all!

-- TEST CODE
health :: Player -> Int
health (Player h) = h

main :: IO()
main = do
    let player = Player 100
    let damagedPlayer = hurtPlayer player
    let difference = health player - health damagedPlayer

    print(difference >= 0) -- how can I retrofit this to allow for variable amounts of damage?


