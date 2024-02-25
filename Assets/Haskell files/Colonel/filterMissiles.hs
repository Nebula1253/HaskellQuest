-- Code snippet starts here
type Position = Int

data Missile = Missile Position Bool -- the bool is whether the missile is active or not

launchMissiles :: Int -> [Missile]
launchMissiles nrMissiles = [Missile i (randomGen i) | i <- [1..nrMissiles]]
-- don't worry about randomGen, it's just a function that randomly decides if the missile is active or not

-- write a function to filter out the missiles that are INACTIVE: we'll be using this to interfere with the attack!
filterMissiles :: [Missile] -> [Missile]
filterMissiles missiles = -- INPUT HERE --


-- TEST CODE
randomGen :: Int -> Bool
randomGen i = False -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

main :: IO()
main = do
    let missiles = [Missile 1 True, Missile 2 False, Missile 3 True, Missile 4 True]
    let newMissiles = filterMissiles missiles
    let highlightCheck = and [b == False | (Missile _ b) <- newMissiles]
    print(highlightCheck)