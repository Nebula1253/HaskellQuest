type Health = Int
data Player = Player Health

damages :: [Int]
damages = [-10,-5,0,5,10]
-- as before, this'll be passed to hurtPlayer

damageFactors :: [Float]
damageFactors = [1, 0.95, 0.9, 0.85, 0.8]
-- as before, this'll be passed to weakenPlayer

-- PLAYER 1 CODE
hurtPlayer :: Player -> [Int] -> Player
hurtPlayer player [] = player
hurtPlayer (Player h) (d:ds)
    | d >= 0    = hurtPlayer (Player (h - d)) ds
    | otherwise = hurtPlayer (Player h) ds

-- The 'sum' function from the previous tutorial was 
-- implemented with recursion!

-- PLAYER 2 CODE
weakenPlayer :: Player -> [Float] -> Player
weakenPlayer player [] = player
weakenPlayer (Player h) (d:ds)
    | d <= 1 = 
        weakenPlayer (Player (round (fromIntegral h * d))) ds
    | otherwise = 
        weakenPlayer (Player h) ds

-- The 'product' function from the previous tutorial was 
-- implemented with recursion!

-- TEST CODE
health :: Player -> Int
health (Player h) = h

percentDiff :: Player -> Player -> Float
percentDiff (Player ogHealth) (Player altHealth)= fromIntegral altHealth / fromIntegral ogHealth

main :: IO()
main = do
    let player = Player 100
    let damagedPlayer = hurtPlayer player damages
    let healthDamage = health player - health damagedPlayer

    let player2 = Player 100
    let damagedPlayer2 = weakenPlayer player2 damageFactors
    let healthPercent = percentDiff player2 damagedPlayer2

    if healthDamage <= 0
        then
            if healthPercent >= 1
                then
                    print "True"
                else
                    print "error: Player 1's function works but Player 2's function doesn't!"
        else
            if healthPercent >= 1
                then
                    print "error: Player 2's function works but Player 1's function doesn't!"
                else
                    print "error: Neither function works as expected!"

    print ("Additional: " ++ show (healthDamage, healthPercent))