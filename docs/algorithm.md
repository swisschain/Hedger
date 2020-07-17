# Source data

* subscribe to trades from account
* subscribe market prices

# Model

**Bucket** - selected base pairs in the context of which trade positions are managed.
Received trades are re-calculated in Buckets.

Params:

| name | description |
| ---- | ----------- |
| `Asset` | asset associated with the basket |
| `QuoteAsset` | asset to which the hedging position is quoted |
| `HadgeTradingPair` | the trading instrument in which the basket position is hedged (`Backet Asset`/`Quote Asset` or `Quote Asset`/`Backet Asset`) |
| `Price` | Current market prices by baccket instrument |

As bucket we will use pairs of the form: Traded asset to USD.

**Trade** - deal on the observable account

Params:

| name | description |
| ---- | ----------- |
| `AssetPair` | trading instrument |
| `BaseAsset` | base asset in trading instrument (BTC for BTC/USD)  |
| `QuoteAsset` | quote asset in trading instrument (USD for BTC/USD) |
| `Price` | trading price |
| `Volume` | trade volume in base asset. Include direction: `sell` - negative value, `buy` - positive value |
| `VolumeOpposite` | trade volume in quote asset. include direction: `sell` - positive value, `buy` - negative value |

# Algorithm to handle incoming trade

### 1. Receive Trade

### 2. Put trade in position list

### 3. WHILE in position list exist position by instrumnet without backet

### 3.1. exclude position

### 3.2. Get backet for **Position.BaseAsset** = BaseBacket

### 3.3. IF (BaseBacket.QuoteAsset == Position.QuoteAsset)

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS ( TradeVolumeOpposite / TradeVolume) | TradeVolume | TradeVolumeOpposite |

### 3.4. IF (BaseBacket.QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBacket[Trade.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS( CrossVolume / TradeVolume ) | TradeVolume | -CrossVolume |
| EURUSD | ABS( -CrossVolume / TradeVolumeOpposite ) | TradeVolumeOpposite | CrossVolume |

### 4. Log change backet and appy change to backet aggregate position



# Examples

## Example 1

Market:

* BTCUSD: bid = 9200 ask = 9210

Backets:

* BTCUSD

---

Trade:

* AssetPair = BTCUSD
* Price = 9210
* volume = 2 (`buy`)
* oppositeVolume = -18420

---

backet update:

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | 9210 | 2 | -18420 |


> IF (Backet[Trade.BaseAsset].QuoteAsset == Trade.quoteAsset)

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS ( TradeVolumeOpposite / TradeVolume) | TradeVolume | TradeVolumeOpposite |


## Example 2

Market:

* BTCUSD: bid = 9200 ask = 9210
* EURUSD: bid = 1.14050 ask = 1.14055
* BTCEUR: bid = 8066 ask = 8075

Backets:

* BTCUSD
* EURUSD

---

Trade:

* AssetPair = BTCEUR
* Price = 8078
* volume = 1.5 (`buy`)
* oppositeVolume = -12117.0

---

backet update:

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | 9213.3629 | 1.5 | -13820.04435 |
| EURUSD | 1.14055 | -12117 | 13820.04435 |


> IF (Backet[Trade.BaseAsset].QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBacket[Trade.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS( CrossVolume / TradeVolume ) | TradeVolume | -CrossVolume |
| EURUSD | ABS( -CrossVolume / TradeVolumeOpposite ) | TradeVolumeOpposite | CrossVolume |




## Example 3

Backets:

* BTCUSD
* ETHUSD
* LKKBTC

---

Trade:

* AssetPair = LKKETH
* Price = 
* volume = 
* oppositeVolume = 


> IF (Backet[Trade.BaseAsset].QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBacket[Trade.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| backet | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| LKKBTC | | | |
| BTCUSD | | | |
| ETHUSD | | | |







