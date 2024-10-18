## Install docker
`curl -sSL https://get.docker.com | sh`


## Install dotnet
`curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS`


## Install dotnet-outdated-tool
`dotnet tool install --global dotnet-outdated-tool`

Tools directory '/home/pi/.dotnet/tools' is not currently on the PATH environment variable.
If you are using bash, you can add it to your profile by running the following command:

cat << \EOF >> ~/.bash_profile
# Add .NET Core SDK tools
export PATH="$PATH:/home/pi/.dotnet/tools"
EOF

You can add it to the current session by running the following command:

export PATH="$PATH:/home/pi/.dotnet/tools"


## Raspberry Pi 4

 - Install Raspbian using NOOBs OS (takes a while after 100% completion).
 - Enable VCN [youtube](https://www.youtube.com/watch?v=YP3_gvHZhfw) or [link](https://howtoraspberrypi.com/raspberry-pi-vnc/) for your Pi:
   ```
   # run the command 
   sudo raspi-config
   # select the line `Interfacing Options`, then line `VNC`, 
   # and finally answer that you want to enable `VNC`
   ```
 - Optional set remote desktop resolution
   ```
   # run the command
   sudo nano /boot/config.txt
   
   # Uncomment the following lines in config.txt
   framebuffer_width=1900
   framebuffer_height=1024
  
   # Comment the follwoing lines (if they are uncommented)
   
   #dtoverlay=vc4-kms-v3d
   #max_framebuffers=2
   ```
   
 - Connect all the components see [diagram](https://tutorials-raspberrypi.de/wp-content/uploads/2014/05/ultraschall_Steckplatine.png)
 - Install **Docker** on the Pi see instructions [here](https://linuxize.com/post/how-to-install-and-use-docker-on-raspberry-pi/)
 - Pull [this](https://github.com/trusharmistry/ForPi_dotnet) git repo:
   ```
   git clone https://github.com/trusharmistry/ForPi_dotnet
   ```
 - We will be using the following two services to a. make our asp.net app publicly visible and be able to respond to sms:
    
   - *ngrok*
     Open an [account](https://ngrok.com). Free for single use.
     Login the ngrok site and will be naviagted to the dashboard page.
     Note down the *token* show as "step 3" on the dashboard page.
     (The ngrok executable file (see folder ngrok-stable-linux-arm) is added to this git repo so no need to download it agian)
   
    - *Twilio*
     Open an [account](https://https://www.twilio.com) if you don't have one.
     Purchase a sms-phone number ($1 per year fee + sms [charges](https://support.twilio.com/hc/en-us/articles/223134687-How-Twilio-charges-for-Short-Code-messages)).
     
 - Build & run the **docker** container: 
   ```
   # build
   docker build -t pi .
   # run (use --privileged to avoid dev/mem error when accessing RPi.GPIO hardware)
   docker run -d --privileged --restart always -p 5000:80 --name forpi pi
   ```

 - Start **ngrok** server:
   ```
   # naviate to ForPi/Pi/ngrok-stable-linux-arm folder
   ./ngrok authtoken <YourAuthenticationTokenFromNgrok>
   
   # start the server
   ./ngrok http 127.0.0.1:5000
   
   # This should show a unique http and https address which 
   # are forwarding request to your local aspnet core service
   # running on Pi
   
   # Add that to your twilio account:
   # - Log into Twilio.com and go to the Console's Numbers page.
   # - Click on your SMS-enabled phone number.
   # - Find the Messaging section. The default “CONFIGURE WITH” is what you’ll need: "Webhooks, TwiML, [etc.]".
   # - In the “A MESSAGE COMES IN” section, select "Webhook" and paste in your URL: in this quickstart step above, it would be: https://123454abc.ngrok.io/sms - be sure to add `/sms` at the end, as this is the route to the SmsController class.
   ```

-------
- Note if need to stop/delete all old containers:
   ```
   docker container stop $(docker container ls -aq);
   docker container rm $(docker container ls -aq)
   ``` 

-------
- Run Seq Server (prefer a x86 machine than Pi since docker image is incompitible for Pi ARM processor):
  ```
  docker run -d --name seq-server --restart always -e "ACCEPT_EULA=Y" -p 5341:80 datalust/seq:latest;
  ```
