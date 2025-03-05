type Depth = Int

data Stability = Inert | Unstable | Healed

data Fractal = NodeFractal Depth Stability (Fractal, Fractal)
                | LeafFractal
-- the Bool in NodeFractal represents whether it's stable
-- or not

createFractal :: Int -> Fractal
createFractal 0 = LeafFractal
createFractal n = NodeFractal n (randomGen n) children
    where children = (createFractal (n - 1), createFractal (n - 1))

-- PLAYER CODE
-- The Fractal structure is unstable, but fixing this can save
-- Dr. Kowalewski! Write a function to traverse the structure:
-- any NodeFractals that are Unstable should be Healed, while
-- the Inert ones should be ignored. Keep the Depth values
-- intact!

healFractal :: Fractal -> Fractal
healFractal fractal = undefined

-- hint: The recursive step needs to behave differently
-- depending on what the Stability is

-- TEST CODE
randomGen :: Int -> Stability
randomGen n = case mod n 2 of
                0 -> Inert
                _ -> Unstable

testGen :: Int -> Stability
testGen n = case mod n 2 of
                0 -> Inert
                _ -> Healed

instance Eq Stability where
    Inert == Inert = True 
    Unstable == Unstable = True 
    Healed == Healed = True
    _ == _ = False
    

instance Eq Fractal where
    LeafFractal == LeafFractal = True
    NodeFractal n s (a, b) == NodeFractal n' s' (c, d) = (n == n') && (s == s') && (a == c) && (b == d)
    _ == _ = False

healthyFractal :: Int -> Fractal
healthyFractal 0 = LeafFractal
healthyFractal n = NodeFractal n (testGen n) children
    where children = (healthyFractal (n - 1), healthyFractal (n - 1))

containsInert :: Fractal -> Bool
containsInert LeafFractal = False 
containsInert (NodeFractal _ s (a, b)) = case s of
                                            Inert -> True 
                                            _ -> containsInert a || containsInert b

containsUnstable :: Fractal -> Bool 
containsUnstable LeafFractal = False 
containsUnstable (NodeFractal _ s (a, b)) = case s of
                                            Unstable -> True 
                                            _ -> containsUnstable a || containsUnstable b

containsHealed :: Fractal -> Bool 
containsHealed LeafFractal = False 
containsHealed (NodeFractal _ s (a, b)) = case s of
                                            Healed -> True 
                                            _ -> containsHealed a || containsHealed b

main :: IO()
main = do
    let ideal = healthyFractal 4
    let check = healFractal (createFractal 4)

    if not (containsHealed check)
        then print "error: The Fractal doesn't contain any Healed nodes! Check your recursive case!"
    else if not (containsInert check)
        then print "error: The Fractal doesn't contain any Inert nodes! Check your recursive case!"
    else if containsUnstable check
        then print "error: The Fractal still contains Unstable nodes!"
    else 
        print (ideal == check)