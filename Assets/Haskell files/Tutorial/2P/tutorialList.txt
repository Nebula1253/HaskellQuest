type Health = Int
data Player = Player Health

damages :: [Int]
damages = [-10, -5, 0, 5, 10] 
-- this'll be passed to hurtPlayer

damageFactors :: [Float]
damageFactors = [1, 0.95, 0.9, 0.85, 0.8]
-- this'll be passed to weakenPlayer

-- PLAYER 1 CODE
hurtPlayer :: Player -> [Int] -> Player
hurtPlayer (Player health) dList = Player (health - damage)
    where damage = sum [d | d <- dList, d >= 0]
    
-- The 'where' clause is quite useful; it allows you to
-- define variables and functions entirely within the scope 
-- of a function. 

-- If you tried to access 'damage' outside of 'hurtPlayer', 
-- it'd throw an error.

-- You know the drill; change the code so you take no damage!

-- PLAYER 2 CODE
weakenPlayer :: Player -> [Float] -> Player
weakenPlayer (Player health) dList = Player newHealth
    where 
        newHealth = round (fromIntegral health * damageFloat)
        damageFloat = product [d | d <- dList, d <= 1]

-- The 'where' clause is quite useful; it allows you to
-- define variables and functions entirely within the scope 
-- of a function. 

-- If you tried to access 'newHealth' or 'damageFloat' outside 
-- of 'hurtPlayer', it'd throw an error.

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