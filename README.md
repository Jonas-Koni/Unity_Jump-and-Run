# Unity_Jump-and-Run

# Basic Overview
Simple platformer game with procedural level Generation. Different Levels focus on school subjects such as Maths with sine waves or physics with moving pendulums.

# Speed Calculation
Each level is assigned a specific speed, which is calculated with an exponential function.

# Random Number Generation
To get a random Number there are two possible Options: You can need a random Number where each number has the same probability. Here you should use the ConstantSpreadNumber() function.
If you want to get a higher chance of getting a number in the middle of your two values there is the PolynomialSpread() function:
$2^{2n}\times(x-\frac{1}{2})^{2n+1} + \frac{1}{2}$ \
The integer variable $n$ is the exponentAmplfier, which results in a higher chance of getting a number in the middle of your set. \
The variable x represents a random Number between 0 and 1, which is generated by the UnityEngine.Random class. \
\
derivation: \
\
![GeoGebra](images-github/formula1.png)
\
\
text \
$f(x) = a(x-\frac{1}{2})^{2n+1}$ \
$f(0) = -1$ \
$a(-\frac{1}{2})^{2n+1} = -1$ \
$a = 2^{2n} + 1$ \
\
move 1 up and scale by factor 2 \
\
$f(x) = 2^{-1} \times (2^{2n+1} \times (x-\frac{1}{2})^{2n+1}+1)$ \
$f(x) = 2^{2n}\times(x-\frac{1}{2})^{2n+1} + \frac{1}{2}$ 

# Level Types
