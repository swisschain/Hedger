# Source data

* subscribe to trades stream from an internal account
* subscribe to market prices stream from an external venues

# Model

**Bucket** - selected base asset pairs in the context of which trade positions are managed.  
Received trades are re-calculated as positions in buckets.

Params:

| name | description |
| ---- | ----------- |
| `Asset` | asset associated with the basket |
| `QuoteAsset` | asset to which the hedging position is quoted |
| `HadgeTradingPair` | the trading instrument in which the basket position is hedged (`Backet Asset`/`Quote Asset` or `Quote Asset`/`Backet Asset`) |
| `Price` | current average price for bucket's asset pair |

As a bucket we use an asset pair in the form of `Asset`/`QuoteAsset`.

**Trade** - an executed deal on the observable account

Params:

| name | description |
| ---- | ----------- |
| `AssetPair` | trading instrument |
| `BaseAsset` | base asset of the trading instrument (BTC for BTC/USD)  |
| `QuoteAsset` | quote asset of the trading instrument (USD for BTC/USD) |
| `Price` | trading price |
| `Volume` | trade volume in base asset. Include direction: `sell` - negative value, `buy` - positive value |
| `VolumeOpposite` | trade volume in quote asset. include direction: `sell` - positive value, `buy` - negative value |

# Algorithm to handle incoming trade

### 1. Receive Trade

### 2. Put the trade into position list

| instrument | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| trade.AssetPair | ABS ( TradeVolumeOpposite / TradeVolume) | trade.Volume | Trade.VolumeOpposite |


### 3. WHILE in position list exists a position with an instrument wich is not bucket

### 3.1. exclude position

### 3.2. Get bucket for **Position.BaseAsset** = BaseBucket

### 3.3. IF (BaseBucket.QuoteAsset == Position.QuoteAsset)

| instrument | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| Bucket[Position.BaseAsset].AssetPair | ABS ( TradeVolumeOpposite / TradeVolume) | TradeVolume | TradeVolumeOpposite |

### 3.4. IF (BaseBucket.QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBucket[Position.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| instrument | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| Bucket[Position.BaseAsset].AssetPair | ABS( CrossVolume / TradeVolume ) | TradeVolume | -CrossVolume |
| Bucket[Position.QuoteAsset].AssetPair | ABS( -CrossVolume / TradeVolumeOpposite ) | TradeVolumeOpposite | CrossVolume |

### 4. Log change bucket and appy change to bucket aggregate position



# Examples

## Example 1

Market:

* BTCUSD: bid = 9200 ask = 9210

Buckets:

* BTCUSD

---

Trade:

* AssetPair = BTCUSD
* Price = 9210
* volume = 2 (`buy`)
* oppositeVolume = -18420

---

bucket update:

| bucket | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | 9210 | 2 | -18420 |


> IF (Bucket[Trade.BaseAsset].QuoteAsset == Trade.quoteAsset)

| bucket | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS ( TradeVolumeOpposite / TradeVolume) | TradeVolume | TradeVolumeOpposite |


## Example 2

Market:

* BTCUSD: bid = 9200 ask = 9210
* EURUSD: bid = 1.14050 ask = 1.14055
* BTCEUR: bid = 8066 ask = 8075

Buckets:

* BTCUSD
* EURUSD

---

Trade:

* AssetPair = BTCEUR
* Price = 8078
* volume = 1.5 (`buy`)
* oppositeVolume = -12117.0

---

bucket update:

| bucket | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | 9213.3629 | 1.5 | -13820.04435 |
| EURUSD | 1.14055 | -12117 | 13820.04435 |


> IF (Bucket[Trade.BaseAsset].QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBucket[Trade.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| bucket | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| BTCUSD | ABS( CrossVolume / TradeVolume ) | TradeVolume | -CrossVolume |
| EURUSD | ABS( -CrossVolume / TradeVolumeOpposite ) | TradeVolumeOpposite | CrossVolume |




## Example 3

Buckets:

* BTCUSD
* ETHUSD
* LKKBTC

---

Trade:

* AssetPair = LKKETH
* Price = 
* volume = 
* oppositeVolume = 


> IF (Bucket[Trade.BaseAsset].QuoteAsset != Trade.quoteAsset)

var CrossVolume = - ( TradeVolumeOpposite * BaseBucket[Trade.QuoteAsset].Price( TradeVolumeOpposite > 0 ? "ask" : "bid" ) );

| bucket | price | volume | oppositeVolume |
| ------ | ----- | ------ | -------------- |
| LKKBTC | | | |
| BTCUSD | | | |
| ETHUSD | | | |


## Example 4

Buckets:

* USDCHF
---

Trade:

* AssetPair = CHFUSD
* Price = 
* volume = 
* oppositeVolume = 




