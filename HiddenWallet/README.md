# Hidden Wallet
Privacy focused, full block downloading SPV Bitcoin wallet with TumbleBit support. This repository is a thin GUI
  
Pizza and/or coffee is welcome: [186n7me3QKajQZJnUsVsezVhVrSwyFCCZ](https://www.smartbit.com.au/address/186n7me3QKajQZJnUsVsezVhVrSwyFCCZ).
    
## Status & Roadmap
1. [Full Block Downloading SPV](https://medium.com/@nopara73/bitcoin-privacy-landscape-in-2017-zero-to-hero-guidelines-and-research-a10d30f1e034) - **Ready, unstable.**  This feature provides full node level privacy against network analysis with SPV security.  
2. [TumbleBit integration, Classic Tumbler Mode](https://hackernoon.com/understanding-tumblebit-part-1-making-the-case-823d786113f3) - **Next up.** TumbleBit is a Bitcoin mixer where not even the Tumbler can steal your coins, nor deanonymize you.   
3. [Transaction Filtered Block Downloading](https://medium.com/@nopara73/full-node-level-privacy-even-for-mobile-wallets-transaction-filtered-full-block-downloading-wallet-16ef1847c21)  - 10-100x performance increase, while still keeping full node level privacy against network analysis, but replacing SPV security to trusted full node security.  
4. Release stable version.  
5. TumbleBit Paymen Hub Mode? JoinMarket integration? Stealth addresses support?

## Configuration

After first running the software, it will generate a `Config.json` file for you:  
```
{
  "WalletFilePath": "Wallets\\Wallet.json",
  "Network": "Main",
  "CanSpendUnconfirmed": "False"
}
```  
For testing, set the network to `"TestNet"` and enable the spending of unconfirmed transactions by setting its value to `"True"`.  
If you've already generated a wallet on the mainnet, then you want to change the default wallet file path, too, for example to `"WalletTestNet.json"`.  
Since testnet coins have no value, you can acquire them freely and quickly: http://lmgtfy.com/?q=get+testnet+faucet

## Building from source code  
  
### Requirements:  
- [Git](https://git-scm.com/downloads)  
- [.NET Core](https://www.microsoft.com/net/core)  
- [Node.js](https://nodejs.org/en/download/)
- Tor: On Linux and OSX make sure "tor" is in your PATH!
  
### Step by step
  
1. copy this project
2. `cd HiddenWallet/HiddenWallet/HiddenWallet.API`  
3. `dotnet restore`  
4. `dotnet build`
5. `dotnet publish -r win7-x64 --output bin/dist/current-target`. Find your platform identifier [here](https://github.com/dotnet/docs/blob/master/docs/core/rid-catalog.md#windows-rids) and replace `win7-x64`.
If you get an error here, don't worry, just add your platform identifier to the `<RuntimeIdentifiers>` tag in the `HiddenWallet.API/HiddenWallet.API.csproj file).  
6. (Only on Windows) - Copy and unzip `HiddenWallet.Packager/tor.zip` to `HiddenWallet.API/bin/dist/current-target` directory. (Quick note on unzipping: the final path to tor.exe has to be: `current-target/tor/Tor/tor.exe` and not: `current-target/tor/tor/Tor/tor.exe`.)
7. `cd ../HiddenWallet.GUI`
8. `npm install`
9. `npm start`
