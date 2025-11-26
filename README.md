# Návrh webové aplikace: Kasino, loterie a sázení

**Autoři:** Matěj Krňávek, Jakub Severin

## Cíl projektu

Vytvořit vícevrstvou webovou aplikaci umožňující uživatelům:

* [x] Registrovat se a spravovat svůj účet. ✅
* [ ] Sázet na sportovní události. 🔄 *(API pro vložení sázky je hotové, chybí data událostí)*
* [x] Hrát základní kasinové hry. ✅ *(Implementovány a plně funkční hry Kostky a Ruleta)*
* [ ] Kupovat losy do loterie. ❌
* [x] Spravovat peněžní zůstatek (wallet). ✅

---

## Funkční požadavky

* **Registrace a přihlášení uživatele** ✅ *(Implementováno přes AccountController a Cookies, včetně validace)*
* **Peněženka a transakce** (vklady, výběry, sázky, výhry) ✅ *(Atomické transakce fungují, zůstatek se aktualizuje v reálném čase)*
* **Správa sázek a zobrazování výsledků** 🔄 *(Sportovní sázení - logika připravena, kasino hry - hotovo)*
* **Správa loterie** (tikety, losování) ❌
* **Záznam kasinových her a výsledků** ✅ *(Výsledky her se zapisují do historie transakcí)*
* **Role uživatelů:** `User`, `Admin` ❌ *(Zatím jen User bez rolí)*
* **Admin správa** kurzů, událostí a losování ❌

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
* `GameSessions` 🔄 *(Historie her je aktuálně řešena přes entitu Transactions)*

### Stručný přehled tabulek

**Users** ✅

| Sloupec | Popis |
| :--- | :--- |
| Id | Primární klíč (int) |
| Username, Email, PasswordHash | Údaje pro autentizaci |
| Role | `User` nebo `Admin` (Zatím nevyužito) |

**Wallets** ✅

| Sloupec | Popis |
| :--- | :--- |
| Balance | Aktuální zůstatek |
| Currency | Měna účtu (Default: EUR/CZK) |

**Transactions** ✅

| Sloupec | Popis |
| :--- | :--- |
| Type | Deposit, Withdrawal, BetStake, BetWin, GameWin, GameLoss |
| Amount | Částka transakce |
| Note | Detail transakce (např. "Dice: Tip 6, Hod 6") |

**Bets / Events** 🔄

* Uživatel vytváří sázky na události. ✅ *(Přes API `BetsController`)*
* Výpočet výsledků probíhá na základě kurzů a výsledku události. ❌ *(Logika připravena, ale chybí data událostí)*

**Lottery a Casino** ✅

* **Lucky Dice:** Uživatel sází na číslo 1-6. Výhra 6x vklad. ✅
* **Ruleta:** Uživatel sází na číslo (36x) nebo barvu (2x). Animace a vyhodnocení funguje. ✅
* Výsledky se ukládají a okamžitě ovlivňují zůstatek uživatele.

### Vztahy mezi entitami

* `User` 1..1 `Wallet` ✅
* `Wallet` 1..* `Transactions` ✅
* `User` 1..* `Bets` ✅
* `Bets` 1..* `BetSelections` ✅
* `Event` 1..* `Markets` ❌
* `Markets` 1..* `Odds` ❌
* `LotteryDraw` 1..* `LotteryTickets` ❌
* `CasinoGame` 1..* `GameSessions` ❌