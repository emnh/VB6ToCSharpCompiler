```bash
# To rebuild the DLL
sudo apt install maven
git clone https://github.com/uwol/proleap-vb6-parser
cd proleap-vb6-parser
# TODO: apply emh.patch for Norwegian letters
mvn clean package

# Download IKVM
#wget https://netix.dl.sourceforge.net/project/ikvm/ikvm/7.2.4630.5/ikvmbin-7.2.4630.5.zip
#unzip ikvmbin-7.2.4630.5.zip
#wget https://github.com/jessielesbian/ikvm/releases/download/8.6.5.1/ikvm_8.6.5.1_bin_windows.zip
#mkdir ikvm8
#cd ikvm8
#unzip ../ikvm*.zip
#cd ..

# First download and install chocolatey (https://chocolatey.org/install), then
choco install ikvm --version=8.1.5717.0

# Get CLASSPATH
mvn dependency:build-classpath -Dmdep.outputFile=cp.txt

# Copy libraries
mkdir libs
cp $(cat cp.txt | sed 's@:@ @g') libs/

# Generate DLL (expect lots of warnings)
#./ikvm-7.2.4630.5/bin/ikvmc.exe -target:library libs/* target/proleap-vb6-parser-2.3.0.jar -out:App.dll
./ikvm8/ikvmc.exe -target:library libs/* target/proleap-vb6-parser-2.3.0.jar -out:App.dll

# Copy
cp App.dll $TARGET/ProLeapParserDLL/App.dll

# Copy IKVM DLLs
cp ikvm8/*.dll $TARGET/ProLeapParserDLL/
```
