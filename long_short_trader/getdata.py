import pandas as pd
import yfinance as yf
from datetime import datetime, timedelta

tickerSymbol = 'AAPL'

tickerData = yf.Ticker(tickerSymbol)

# df = tickerData.history(period='1d', start='2022-12-10', end='2023-12-10', interval='1m')

# print(df.head())

# df[['Open']].to_csv('C:\\Development\\research\\Stock-trading-bot-research\\trading-processor\\data\\output.csv', index=True)  

# Define the start and end dates
start_date = datetime.strptime('2023-12-01', '%Y-%m-%d')
end_date = datetime.strptime('2023-12-20', '%Y-%m-%d')

# Initialize an empty DataFrame to store all data
# all_data = pd.DataFrame()
all_data = None

counter = 0

while start_date < end_date:
    print('Week: ' + str(counter))
    # Get the end date for the current week
    week_end_date = start_date + timedelta(days=7)
    
    # Ensure the week_end_date does not exceed the end_date
    if week_end_date > end_date:
        week_end_date = end_date
    
    # Get the historical data for the current week
    df = tickerData.history(period='1d', start=start_date.strftime('%Y-%m-%d'), end=week_end_date.strftime('%Y-%m-%d'), interval='1m')
    
    if all_data is None:
        all_data = df
    else:
        # Append the data to the all_data DataFrame
        all_data = all_data.append(df)
    
    # Move to the next week
    start_date = week_end_date

    counter = counter + 1

all_data[['Open']].to_csv('C:\\Development\\research\\Stock-trading-bot-research\\trading-processor\\data\\output.csv', index=True)