-- Code snippet starts here
type Position = Int

data Missile = Missile Position Bool -- the bool is whether the missile is active or not

launchMissiles :: Int -> [Missile]
launchMissiles nrMissiles = [Missile i (randomGen i) | i <- [1..nrMissiles]]
-- don't worry about randomGen, it's just a function that randomly decides if the missile is active or not

-- write a function to filter out the missiles that are INACTIVE, and then highlight them: we'll be using this to interfere with the attack!
-- You'll have to use the function highlight :: Missile -> Missile here; don't worry about how it works, just make sure you highlight the right missiles
highlightDummies :: [Missile] -> [Missile]
highlightDummies missiles = -- INPUT HERE --


-- TEST CODE
randomGen :: Int -> Bool
randomGen i = False -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

highlight :: Missile -> Missile
highlight missile = missile -- who cares lol the player's not gonna see this, the definition is just here to stop a compiler error

main :: IO()
main = do
    let missiles = [Missile 1 True, Missile 2 False, Missile 3 True, Missile 4 True]
    let newMissiles = highlightDummies missiles
    let highlightCheck = and [b == False | (Missile _ b) <- newMissiles]
    print(highlightCheck)
