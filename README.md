# Návrh webové aplikace: Kasino, loterie a sázení

**Autoři:** Matěj Krňávek, Jakub Severin

## Cíl projektu

Vytvořit vícevrstvou webovou aplikaci umožňující uživatelům:

* [x] Registrovat se a spravovat svůj účet. ✅
* [ ] Sázet na sportovní události. 🔄 *(API pro vložení sázky je hotové, chybí data událostí)*
* [ ] Hrát základní kasinové hry. ❌
* [ ] Kupovat losy do loterie. ❌
* [x] Spravovat peněžní zůstatek (wallet). ✅

---

## Funkční požadavky

* **Registrace a přihlášení uživatele** ✅ *(Implementováno přes AccountController a Cookies)*
* **Peněženka a transakce** (vklady, výběry, sázky, výhry) ✅ *(Atomické transakce fungují)*
* **Správa sázek a zobrazování výsledků** 🔄 *(Logika pro uložení sázky existuje, vyhodnocení zatím chybí)*
* **Správa loterie** (tikety, losování) ❌
* **Záznam kasinových her a výsledků** ❌
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
* `Game` ✅ *(Entita existuje v Domain)*
* `GameSessions` ❌

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
| Type | Deposit, Withdrawal, BetStake, BetWin |
| Amount | Částka transakce |

**Bets / Events** 🔄

* Uživatel vytváří sázky na události. ✅ *(Přes API `BetsController`)*
* Výpočet výsledků probíhá na základě kurzů a výsledku události. ❌ *(Logika připravena, ale chybí data událostí)*

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