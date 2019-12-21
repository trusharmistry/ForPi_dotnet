## History

Inspired by the agrumnets between my wife and I, whether the garge door is closed after we leave the subdivision, I thought about adding an IoT device. This project is a child of our thoughts put into action. It is designed to measure the distance from my celing to my garage door and checks if it exceeds certain configurable threashold value. 

I am running an `aspnet core` website on Pi with an endpoint hooked with twilio service that responds to incoming query and returns a status, open or close with measured distance in `cm`, for the garage door. 

## Hardware
Hardware used (not a minimum requirement)
- [Raspberry Pi 4](https://www.amazon.com/gp/product/B07YRSYR3M/ref=ppx_yo_dt_b_asin_title_o03_s00?ie=UTF8&psc=1)
- [HC-SR04](https://www.amazon.com/gp/product/B01COSN7O6/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1)
- [2 Ohm Resistor](https://www.amazon.com/gp/product/B07C91S45M/ref=ppx_yo_dt_b_asin_title_o01_s00?ie=UTF8&psc=1)
- [1 Ohm Resistor](https://www.amazon.com/gp/product/B0185FIJ9A/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1)
- [Connectors](https://www.amazon.com/gp/product/B01EV70C78/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1)
- [BreadBoard](https://www.amazon.com/gp/product/B01EV640I6/ref=ppx_yo_dt_b_asin_title_o01_s01?ie=UTF8&psc=1)

## Pi Setup

 - Install Raspbian using NOOBs OS (takes a while after 100% completion).
 - Enable VCN [youtube](https://www.youtube.com/watch?v=YP3_gvHZhfw) or [link](https://howtoraspberrypi.com/raspberry-pi-vnc/) for your Pi:
   ```
   # run the command 
   sudo raspi-config
   
   # select the line `Interfacing Options`, then line `VNC`, 
   # and finally answer that you want to enable `VNC`
   ```
 - Connect all the components see [diagram](https://tutorials-raspberrypi.de/wp-content/uploads/2014/05/ultraschall_Steckplatine.png)
 - Install **Docker** on the Pi see instructions [here](https://linuxize.com/post/how-to-install-and-use-docker-on-raspberry-pi/)
 - Pull [this](https://github.com/trusharmistry/ForPi_dotnet) git repo:
   ```
   git clone https://github.com/trusharmistry/ForPi_dotnet
   ```
 - We will be using the following two services to (a) Make our asp.net app publicly visible (b) Be able to respond to sms:
    
   - *ngrok*
     Open an [account](https://ngrok.com). Free for single use.
     Login the ngrok site and will be naviagted to the dashboard page.
     Note down the *token* show as "step 3" on the dashboard page.
     (The ngrok executable file (see folder ngrok-stable-linux-arm) is added to this git repo so no need to download it agian. Additionally, please secure your `ngrok` server before exposing to internet see [documentation](https://ngrok.com/docs) for details.)
   
    - *Twilio*
     Open an [account](https://https://www.twilio.com) if you don't have one.
     Purchase a sms-phone number ($1 per year fee + sms [charges](https://support.twilio.com/hc/en-us/articles/223134687-How-Twilio-charges-for-Short-Code-messages)).
     
 - Build & run the **docker** container: 
   Note: The `appsettings.json` contains one configuration value for minimum distance to my garage door.
   ```
   # change to `Pi` directory (assuming that you cloned to `git` directory)
   cd ~/git/ForPi_dotnet/Pi
   
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

---

- **Pi Installed**
<img src="https://github.com/trusharmistry/ForPi_dotnet/blob/master/imagesForReadMe/distanceToDoor.jpg" width="400">

- **Twilio Response**
<img src="https://github.com/trusharmistry/ForPi_dotnet/blob/master/imagesForReadMe/twlioResponse.jpg" width="400">

-------

- Note if need to stop/delete all old containers:
   ```
   docker container stop $(docker container ls -aq);
   docker container rm $(docker container ls -aq)
   ``` 

-------

- Note about running Serilog/Seq Server
  - Use env variable `SEQ_URL` or it will default to `http://localhost:5341`.
  - Prefer a x86 machine than Pi since the docker image below seems to be incompitible for Pi's ARM processor)
  ```
  docker run -d --name seq-server --restart always -e "ACCEPT_EULA=Y" -p 5341:80 datalust/seq:latest;
  ```
  
