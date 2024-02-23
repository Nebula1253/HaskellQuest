type Health = Int
data Player = Player Health

hurtPlayer :: Player -> [Int] -> Player
hurtPlayer (Player health) dList = Player (health - damage)
    where damage = sum [d | d <- dList, d >= 0]
-- The 'where' clause is quite useful; it allows you to
-- define variables and functions entirely within the scope of a function.
-- If you tried to access 'damage' outside of 'hurtPlayer', it'd throw an error.

damages :: [Int]
damages = [-10,-5,0,5,10] 
-- this'll be passed to hurtPlayer

-- You know the drill; change the code so you take no damage whatsoever!

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