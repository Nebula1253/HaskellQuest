-- this allows you to define custom types as aliases for other types
type Health = Int

-- this defines a custom data structure with a specific pattern
data Player = Player Health 

hurtPlayer :: Player -> Player
hurtPlayer (Player health) = Player (health - 10) 

-- The "(Player health)" there is an example of pattern-matching.
-- You can specify patterns that data should match to, and then 
-- deconstructing that data if it does, in one fell swoop.

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
    print("Additional: " ++ show difference);