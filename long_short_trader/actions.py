import yfinance as yf
from bot import Bot

trades = []
def load_data(ticker: str):
    tickerData = yf.Ticker(ticker)
    tickerDf = tickerData.history(period='1d', start='2023-12-10', end='2023-12-17', interval = '1m')

# def try_trade():
    

# def simulate(data, cash: int):
#     global trades
#     trades = [] #clear trades
#     current_index = 0
#     while current_index < len(data):
#         current_timestamp = data.index[current_index]


#         current_index += 1