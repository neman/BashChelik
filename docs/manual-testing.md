# Manual testing with a real card

The automatic tests run against a simulated chip. Reading a real document is tested by hand with the console tool, on each system the library supports. Everything the tool sends to the card is a read: no PIN is asked for and nothing is written.

The tool prints the document's personal data on screen. Keep that output to yourself: do not paste it into issues, pull requests, logs or chats, and never commit it.

## The console tool

From the repository root, with a card reader connected and a document in it:

```shell
dotnet run --project src/BashChelik.ConsoleApp -c Release -- read
```

- `read` prints the document through the public API. Compare it with the printed document, especially letters such as `Ž`, `Ć` and `Đ`, and the dates.
- Run it once more and pull the card out while it reads: it must end with `CardRemoved`, not a crash.
- `read --portrait <file.jpg>` also saves the photo, to look at it. Save it outside the repository, for example `C:Tempportrait.jpg` or `/tmp/portrait.jpg`; `*.jpg` is ignored by git all the same.
- `probe` prints only the chip's structure, without any value from the document. It is for checking the chip format.
- Both take an optional reader name; without one they use the first reader.

## Windows

Nothing to install beyond the .NET 11 SDK; Windows has PC/SC built in. Run the tool as above.

## Ubuntu on Windows (WSL 2)

Tried on Ubuntu running on Windows 11 under WSL 2, with a Gemplus USB reader. Linux installed directly on a computer has not been tried; there the reader is plugged in directly and the usbipd steps are skipped. The reader is passed from Windows into WSL with usbipd-win; while it is attached to WSL, Windows does not see it.

### Once, in Windows (PowerShell as administrator)

```powershell
winget install --exact dorssel.usbipd-win
usbipd list
```

Find the card reader in the list (for example "USB Smart Card Reader") and note its `BUSID`, such as `2-3`. Then share it:

```powershell
usbipd bind --busid <BUSID>
```

### Once, in Ubuntu

```bash
sudo apt update
sudo apt install -y pcscd pcsc-tools libccid usbutils
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 11.0
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc && source ~/.bashrc
git clone https://github.com/neman/BashChelik.git ~/BashChelik
```

Clone the repository inside WSL rather than using the Windows checkout through `/mnt/c`: the Windows and Linux builds would otherwise share the same `bin` and `obj` folders.

### Each time

1. In Windows, with WSL running, attach the reader:

   ```powershell
   usbipd attach --wsl --busid <BUSID>
   ```

2. In Ubuntu, start the PC/SC daemon if it is not running. A WSL without systemd cannot use `systemctl` (it answers "System has not been booted with systemd"), so start it by hand; it runs until WSL shuts down:

   ```bash
   lsusb                          # the reader must be listed
   pgrep -a pcscd || sudo pcscd
   pcsc_scan                      # put the card in; it must show an ATR. Stop with Ctrl+C
   ```

3. Run the tool:

   ```bash
   cd ~/BashChelik && git pull
   dotnet run --project src/BashChelik.ConsoleApp -c Release -- read
   ```

4. In Windows, give the reader back:

   ```powershell
   usbipd detach --busid <BUSID>
   ```

### When it does not work

- **`pcsc_scan` or the tool says access is denied.** Some builds of pcscd ask polkit whether a user may use the reader, and polkit needs systemd. Recent versions have `pcscd --disable-polkit`; the version tried here does not have the option and did not need it. Otherwise run the tool as root, keeping your `PATH`:

  ```bash
  sudo env "PATH=$PATH" dotnet run --project src/BashChelik.ConsoleApp -c Release -- read
  ```

- **`pcsc_scan` sees no reader.** Stop the daemon and run it in the foreground in a second terminal to see what it reports when the reader is attached:

  ```bash
  sudo pkill pcscd
  sudo pcscd -f -d
  ```

- **You would rather use systemd.** Add `[boot]` and `systemd=true` to `/etc/wsl.conf`, run `wsl --shutdown` in Windows, and start Ubuntu again; then `sudo systemctl start pcscd` works. This changes the whole distribution, not only this test.

## macOS

Not tried yet. macOS has PC/SC built in, so the tool should run as on Windows.
