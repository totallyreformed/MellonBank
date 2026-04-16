# MellonBank - Test Cases

## Customer Portal — Transfers & Deposits

| Test Case | Expected Result | Status |
|---|---|---|
| Transfer to your own account number | Balance unchanged (subtracts and adds same amount) | Pass |
| Transfer with exactly the full balance (balance becomes 0.00) | Transfer succeeds, balance is 0.00 | Pass |
| Transfer with minimum valid amount (0.01) | Transfer succeeds | Pass |
| Transfer with amount exceeding balance | Error: "Insufficient funds" | Pass |
| Transfer to a non-existent account number | Error: "Destination account not found" | Pass |
| Deposit a negative amount | Validation rejects it | Pass |
| Transfer a negative amount | Validation rejects it | Pass |
| Decimal precision edge case (0.009999999999999999999) | Rounded to 0.01 by SQL Server decimal(18,2) | Pass |
| Deposit extreme value (99999999999999999.99) | Validation error: amount exceeds maximum | Pass |
| Transfer extreme value (99999999999999999.99) | Validation error: amount exceeds maximum | Pass |
| Deposit that would push balance over decimal max | Error: "deposit would exceed maximum allowed balance" | Pass |
| Transfer that would push destination over decimal max | Error: "transfer would exceed destination maximum balance" | Pass |

## Customer Portal — Balance

| Test Case | Expected Result | Status |
|---|---|---|
| Check balance with invalid/expired fixer.io API key | Graceful error message, no crash | Pass |
| Check balance on account with 0.00 balance | Displays 0.00 EUR and 0.00 USD | Pass |

## Staff Panel — Customer Management

| Test Case | Expected Result | Status |
|---|---|---|
| Create customer with duplicate AFM | Error: "A customer with this AFM already exists" | Pass |
| Create customer with duplicate email | Error: "A user with this email already exists" | Pass |
| Create customer with duplicate phone | Error: "A user with this phone number already exists" | Pass |
| Edit customer AFM to one that already exists | Error: "A customer with this AFM already exists" | Pass |
| Edit customer email to one that already exists | Error: "A user with this email already exists" | Pass |
| Edit customer phone to one that already exists | Error: "A user with this phone number already exists" | Pass |
| Edit customer and change nothing | Saves without errors | Pass |
| Delete customer who has accounts | Customer and all their accounts deleted | Pass |
| Access deleted customer's account details URL directly | Returns NotFound | Pass |

## Staff Panel — Account Management

| Test Case | Expected Result | Status |
|---|---|---|
| Create account with duplicate account number | Error: "An account with this number already exists" | Pass |
| Create account with non-existent customer AFM | Error: "Customer with this AFM not found" | Pass |
| Create account with negative balance | Validation error: balance must be 0 or greater | Pass |
| Edit account number to one that already exists | Error: "An account with this number already exists" | Pass |
| Edit account balance to negative | Validation error: balance must be 0 or greater | Pass |
| Edit account number — verify originator gets renamed (not destination overwritten) | Original account renamed correctly, no other account affected | Pass |
| Edit account and change nothing | Saves without errors | Pass |

## Authorization — URL Manipulation

| Test Case | Expected Result | Status |
|---|---|---|
| Customer accesses /Staff/Customers | Access denied | Pass |
| Staff accesses /Customer/Index | Access denied | Pass |
| Unauthenticated user accesses /Staff/Customers | Redirected to login | Pass |
| Unauthenticated user accesses /Customer/Index | Redirected to login | Pass |
| Customer accesses /Customer/AccountDetails/{fake_id} | Returns NotFound | Pass |
| Customer tries to access another customer's account by guessing ID | Returns NotFound (ownership check) | Pass |
| Bookmarked page after session expires | Redirected to login | Pass |

## Registration

| Test Case | Expected Result | Status |
|---|---|---|
| Register with duplicate AFM | Error: "A user with this AFM already exists" | Pass |
| Register with duplicate phone | Error: "A user with this phone number already exists" | Pass |
| Register with duplicate email | Error from Identity: duplicate username | Pass |
| Register without selecting a role | Validation error: "Please select role" | Pass |
| Register with weak password (short, no uppercase, no digit) | Identity password policy errors | Pass |

## Staff-Created Customer Login

| Test Case | Expected Result | Status |
|---|---|---|
| Login as customer created through admin portal | Login succeeds (email confirmed automatically) | Pass |
| Login as customer created through register page | Login succeeds | Pass |