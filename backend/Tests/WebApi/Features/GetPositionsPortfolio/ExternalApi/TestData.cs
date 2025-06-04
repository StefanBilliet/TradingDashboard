namespace Tests.WebApi.Features.GetPositionsPortfolio.ExternalApi;

public static class TestData
{
    public static readonly string SpyBullCall =
        """
        {
            "__count": 2,
            "Data": [
                {
                    "DisplayAndFormat": {
                        "Currency": "USD",
                        "Decimals": 2,
                        "Description": "SPDR S&P 500 ETF Trust Jul2025 587 C",
                        "Format": "Normal",
                        "StrikeDecimals": 2,
                        "StrikeFormat": "Normal",
                        "Symbol": "SPY/11N25C587:xcbf",
                        "UnderlyingInstrumentDescription": "SPDR S&P 500 ETF Trust"
                    },
                    "NetPositionId": "49862353__CO__S",
                    "PositionBase": {
                        "AccountId": "99999999",
                        "AccountKey": "abc123",
                        "Amount": -1.0,
                        "AssetType": "StockOption",
                        "CanBeClosed": true,
                        "ClientId": "99999999",
                        "CloseConversionRateSettled": false,
                        "CorrelationKey": "25c8f0e8-f432-4ab2-8c6b-e351b282c42c",
                        "ExecutionTimeOpen": "2025-06-02T14:27:29.144000Z",
                        "IsForceOpen": false,
                        "IsMarketOpen": false,
                        "LockedByBackOffice": false,
                        "OpenBondPoolFactor": 1.0,
                        "OpenPrice": 13.95,
                        "OpenPriceIncludingCosts": 13.9195,
                        "OptionsData": {
                            "CanBeExercised": false,
                            "ExerciseStyle": "American",
                            "ExpiryCut": "None",
                            "ExpiryDate": "2025-07-11T00:00:00.000000Z",
                            "PutCall": "Call",
                            "SettlementStyle": "PhysicalDelivery",
                            "Strike": 587.0
                        },
                        "RelatedOpenOrders": [],
                        "SourceOrderId": "5035095572",
                        "Status": "Open",
                        "Uic": 49862353,
                        "ValueDate": "2025-06-02T00:00:00.000000Z"
                    },
                    "PositionId": "5023725059",
                    "PositionView": {
                        "CalculationReliability": "ApproximatedPrice"
                    }
                },
                {
                    "DisplayAndFormat": {
                        "Currency": "USD",
                        "Decimals": 2,
                        "Description": "SPDR S&P 500 ETF Trust Jul2025 586 C",
                        "Format": "Normal",
                        "StrikeDecimals": 2,
                        "StrikeFormat": "Normal",
                        "Symbol": "SPY/11N25C586:xcbf",
                        "UnderlyingInstrumentDescription": "SPDR S&P 500 ETF Trust"
                    },
                    "NetPositionId": "49862358__CO__L",
                    "PositionBase": {
                        "AccountId": "99999999",
                        "AccountKey": "abc123",
                        "Amount": 1.0,
                        "AssetType": "StockOption",
                        "CanBeClosed": true,
                        "ClientId": "99999999",
                        "CloseConversionRateSettled": false,
                        "CorrelationKey": "25c8f0e8-f432-4ab2-8c6b-e351b282c42c",
                        "ExecutionTimeOpen": "2025-06-02T14:27:29.144000Z",
                        "IsForceOpen": false,
                        "IsMarketOpen": false,
                        "LockedByBackOffice": false,
                        "OpenBondPoolFactor": 1.0,
                        "OpenPrice": 14.64,
                        "OpenPriceIncludingCosts": 14.6705,
                        "OptionsData": {
                            "CanBeExercised": true,
                            "ExerciseStyle": "American",
                            "ExpiryCut": "None",
                            "ExpiryDate": "2025-07-11T00:00:00.000000Z",
                            "PutCall": "Call",
                            "SettlementStyle": "PhysicalDelivery",
                            "Strike": 586.0
                        },
                        "RelatedOpenOrders": [],
                        "SourceOrderId": "5035095571",
                        "Status": "Open",
                        "Uic": 49862358,
                        "ValueDate": "2025-06-02T00:00:00.000000Z"
                    },
                    "PositionId": "5023725057",
                    "PositionView": {
                        "CalculationReliability": "ApproximatedPrice"
                    }
                }
            ]
        }
        """;
}