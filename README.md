# Assassinate Mod for Mount & Blade II: Bannerlord

This mod adds the ability to assassinate nobles in Town and Castle Keeps.

A successful assassination will harbour no ill effects, unlike executing captured nobles. Getting it wrong, however, will bring dire consequences.

## Building from source
To build this mod from source (on windows), you need to:

* Update the references to point to your local Bannerlord DLLs
* Update `Project > properties > Build > Output path` to be
    ```
    {THE PATH TO YOUR BANNERLORD LOCATION}\Mount & Blade II Bannerlord\Modules\OsbarMods.Assassinate\bin\Win64_Shipping_Client\
    ```
* Update `properties > Debug > Start external program` to be the path to your Bannerlord.exe
* Update `properties > Debug > Command line arguments` to load the modules of your choice
    ```
    /singleplayer _MODULES_*Native*SandBoxCore*CustomBattle*SandBox*StoryMode*YOUR_MODULES_HERE*OsbarMods.Assassinate*_MODULES_
    ```
    e.g.
    ```
    /singleplayer _MODULES_*Native*SandBoxCore*CustomBattle*SandBox*StoryMode*DeveloperConsole*OsbarMods.Assassinate*_MODULES_
    ```
* Update `properties > Debug > Working directory` to be
    ```
    {THE PATH TO YOUR BANNERLORD LOCATION}\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\
    ```
* Add a file called Cfg.bat to the root folder, with the variable `BANNERLORD_DIR` set to be the path to your Bannerlord directory
    ```
    set BANNERLORD_DIR="{THE PATH TO YOUR BANNERLORD LOCATION}"
    ```