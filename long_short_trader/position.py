class Position:
    def __init__(self, ticker: str, amount: int, start_timestamp, start_price: float, target_price: float, stoploss_price: float, type: str):
        self.ticker = ticker
        self.amount = amount
        self.start_timestamp = start_timestamp
        self.start_price = start_price
        self.target_price = target_price
        self.stoploss_price = stoploss_price
        self.type = type
        self.new_position = 0.015
    
    def try_action(self, current_stock_price: float) -> str:
        if self.type == 'long':
            if current_stock_price >= self.target_price:
                return 'SELL'
            if current_stock_price <= self.stoploss_price:
                return 'SELL'
            return 'HOLD'
        if self.type == 'short':
            if current_stock_price <= self.target_price:
                return 'SELL'
            if current_stock_price >= self.stoploss_price:
                return 'SELL'
            return 'HOLD'
    
    def require_new_position(self, current_stock_price: float, new_long_price: float) -> float:
        # no short support yet
        if self.type == 'long':
            if  current_stock_price >= self.start_price * (1 + self.new_position):
                if self.start_price * (1 + self.new_position) > new_long_price:
                    new_long_price = self.start_price * (1 + self.new_position)

        return new_long_price