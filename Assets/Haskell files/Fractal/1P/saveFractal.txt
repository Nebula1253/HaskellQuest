type Name = String

data Person = Person Name | Fractal (Person)

-- This is what Dr. Kowalewski did to herself to become a Fractal
createFractal :: Person -> Int -> Person
createFractal (Person p) 0 = Person p
createFractal p n = Fractal (createFractal p (n-1))

-- Write a function to save Kowalewski from the recursion she's
-- trapped in!
saveFractal :: Person -> Person
saveFractal fractal = --INPUT HERE--

-- TEST CODE
instance Eq Person where
    (Person a) == (Person b) = a == b
    (Fractal a) == (Fractal b) = a == b
    _ == _ = False

main :: IO()
main = do
    let kowalewski = Person "Kowalewski"
    let fractal = createFractal kowalewski 5
    print(saveFractal fractal == kowalewski)