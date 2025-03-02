data Bolt = IntBolt Int |
            FloatBolt Float |
            NodeBolt (Bolt, Bolt) 

launchBolt :: Int -> Bolt
-- the argument is the number of NodeBolt layers in the tree
launchBolt 1 = NodeBolt (IntBolt 10, FloatBolt 0.9)
launchBolt n = NodeBolt (launchBolt (n - 1), launchBolt (n - 1))

data Fractal = DecoyFractal |
                BoltFractal Bolt |
                NodeFractal (Fractal, Fractal)

createFractal :: Int -> Int -> Fractal
-- the first argument is the number of NodeFractal layers in the tree,
-- the second argument is the argument supplied to launchBolt
createFractal 1 n = NodeFractal (DecoyFractal, BoltFractal (launchBolt n))
createFractal f n = NodeFractal (createFractal (f-1) n, createFractal (f-1) n)

-- PLAYER 1 CODE
-- Dr Fractal's creating an energy bolt that splits itself into a tree!

-- write a function to traverse the tree, find the bolts that do damage,
-- and negate them!

disableBolts :: Bolt -> Bolt
disableBolts b = undefined 

-- hint: you'll need three cases, one for each Bolt constructor

-- PLAYER 2 CODE
-- Dr Fractal's split herself into a tree of Fractals! Only some Fractals 
-- in the tree are BoltFractals; they're the ones that'll fire!

-- Write a function to traverse the tree of fractals until you find the
-- BoltFractal; once you have, apply player 1's 'disableBolts' function
-- to the bolt within!

traverseFractals :: Fractal -> Fractal
traverseFractals f = undefined

-- hint: you'll need three cases, one for each Fractal constructor

-- TEST CODE


