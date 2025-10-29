# Návrh webové aplikace: Kasino, loterie a sázení

Autoři: Matěj Krňávek, Jakub Severin

## Cíl projektu

Vytvořit vícevrstvou webovou aplikaci umožňující uživatelům:

* Registrovat se a spravovat svůj účet. ❌
* Sázet na sportovní události. ❌
* Hrát základní kasinové hry. ❌
* Kupovat losy do loterie. ❌
* Spravovat peněžní zůstatek (wallet). ✅



---

## Funkční požadavky

* Registrace a přihlášení uživatele ❌
* Peněženka a transakce (vklady, výběry, sázky, výhry) ✅
* Správa sázek a zobrazování výsledků ❌
* Správa loterie (tikety, losování) ❌
* Záznam kasinových her a výsledků ❌
* Role uživatelů: `User`, `Admin` ❌
* Admin správa kurzů, událostí a losování ❌

---

## Návrh databáze

### Hlavní entity

* `Users` ✅
* `Wallets` ✅
* `Transactions` ✅
* `Bets` ✅
* `BetSelections` ✅
* `Events` ❌
* `Markets` ❌
* `Odds` ❌
* `LotteryDraws` ❌
* `LotteryTickets` ❌
* `Game` ✅
* `GameSessions` ❌

### Stručný přehled tabulek

**Users** ✅

| Sloupec | Popis |
| :--- | :--- |
| Id | Primární klíč |
| Username, Email, PasswordHash | Údaje pro autentizaci |
| Role | `User` nebo `Admin` |

**Wallets** ✅

| Sloupec | Popis |
| :--- | :--- |
| Balance | Aktuální zůstatek |
| Currency | Měna účtu |

**Transactions** ✅

| Sloupec | Popis |
| :--- | :--- |
| Type | Deposit, Withdrawal, BetStake, BetWin |
| Amount | Částka transakce |

**Bets / Events** ❌

* Uživatel vytváří sázky na události.
* Výpočet výsledků probíhá na základě kurzů a výsledku události.

**Lottery a Casino** ❌

* Uživatel může koupit tiket nebo hrát hru.
* Ukládá se výsledek a případná výhra.

### Vztahy mezi entitami

* `User` 1..1 `Wallet` ✅
* `Wallet` 1..* `Transactions` ✅
* `User` 1..* `Bets` ✅
* `Bets` 1..* `BetSelections` ✅
* `Event` 1..* `Markets` ❌
* `Markets` 1..* `Odds` ❌
* `LotteryDraw` 1..* `LotteryTickets` ❌
* `CasinoGame` 1..* `GameSessions` ❌