type Health = Int
data Player = Player Health

hurtPlayer :: Player -> [Int] -> Player
hurtPlayer player [] = player
hurtPlayer (Player health) (d:ds) = hurtPlayer (Player (health - d)) ds
-- you might have realised that this is basically doing the same thing
-- as the previous script; in fact, this is how the 'sum' function from there
-- is implemented.

damages :: [Int]
damages = [-10,-5,0,5,10]
-- as before, this'll be passed to hurtPlayer

-- TEST CODE
health :: Player -> Int
health (Player h) = h

main :: IO()
main = do
    let player = Player 100
    let damagedPlayer = hurtPlayer player damages
    let difference = health player - health damagedPlayer

    print(difference <= 0) -- how can I retrofit this to allow for variable amounts of damage?
    print("Additional: " ++ show difference);