# NBKR
Библиотека для получения ежедневных официальных курсов по USD, EUR, RUB, KZT

Пример :
```cs
 var nbkrRate = new NBKRRate();
// Вернет объект с курсами валют USD, EUR, RUB, KZT
var rates = nbkrRate.GetDailyRate();
Console.WriteLine(rates);
```