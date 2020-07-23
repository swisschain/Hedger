if we sold 2 BTC and bought 1 BTC - our position is long for 1 BTC

if we sold 2 BTC and bought 4 BTC - our position is short for 2 BTC


## Example 1

we sold 1 BTC for $10.000, so we have to buy 1 BTC (long position)

we begin to monitor prices of the BTC/USD and updating the lowest position

BTC/USD is declining $9.900, $9.800, $9.700

trailingStop is $100

BTC/USD started to grow - $9.720, $9.740, ..., $9.800

at this point trailingStop triggers a hedge trade

currentPrice - lowestPrice >= trailingStop

9.800 - 9.700 >= 100

so we have to buy 1 BTC for 9.800
