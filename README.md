# Kassasystemet
---

Inlämningsuppgift "Programmering fördjupning"

--- 

Kassasystem – Objektorienterad kod

### Allmänt
Programmera ett **kassasystem** – som man har i kassan i matbutiken

### Exempel

Produkter i kassasystemet lagras i en fil. Följande data ska lagras på Produkt:
	produktid (snabbkommando i kassan, ex ”300” för bananer nedan)
	pris
	pris typ – är det per kilo eller per styck
	produktnamn

När man kör kassan ska det se ut ungefär som nedan:

| Kassa      |
| ---------- |
| 1. Ny kund |
| 0. Avsluta | 


Vid val av 1 så startas kassan med en ny försäljning.

Här finns två kommandon:

<produktid> <antal> ex 300 1, betyder lägg till en av produktid

PAY = vi ”fejkar” att det betalas och kvittot sparas ned (se nedan) och vi kommer tillbaka till menyn


