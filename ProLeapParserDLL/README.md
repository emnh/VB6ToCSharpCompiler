# To rebuild the DLL
sudo apt install maven
git clone https://github.com/uwol/proleap-vb6-parser
cd proleap-vb6-parser
# TODO: apply emh.patch for Norwegian letters
mvn clean package

# Download IKVM
wget https://netix.dl.sourceforge.net/project/ikvm/ikvm/7.2.4630.5/ikvmbin-7.2.4630.5.zip
unzip ikvmbin-7.2.4630.5.zip
./ikvm-7.2.4630.5/bin/ikvmc.exe target/proleap-vb6-parser-2.3.0.jar

# Copy
cp target/proleap-vb6-parser-2.3.0.dll $TARGET/ProLeapParserDLL/App.dll

# TODO: Question? Why is the old DLL 18 MB and the new one is 600k? Missing something / debug version?
