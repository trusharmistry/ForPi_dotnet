## Raspberry Pi 4

 - Install Raspbian using NOOBs OS (takes a while after 100% completion).
 - Enable VCN [youtube](https://www.youtube.com/watch?v=YP3_gvHZhfw) or [link](https://howtoraspberrypi.com/raspberry-pi-vnc/) for your Pi
   ```
   # run the command 
   sudo raspi-config
   # select the line Interfacing Options, then line VNC, 
   # and finally answer that you want to enable VNC
   ```
 - Connect all the components see [diagram](https://tutorials-raspberrypi.de/wp-content/uploads/2014/05/ultraschall_Steckplatine.png)
 - Install Docker on the Pi see instructions [here](https://linuxize.com/post/how-to-install-and-use-docker-on-raspberry-pi/)
 - Pull [this](https://github.com/iamtrushar/ForPi) git repo
 - We will be using the following two services to get this going:
    
   - *ngrok*
     Open an [account](https://ngrok.com). Free for single use.
     Login the ngrok site and you will see the dashboard page.
     The ngrok file is already added to the git repo so no need to download it agian.
     Note down the *token* show as "step 3" on the dashboard page.
   
    - *Twilio*
     Open an [account](https://https://www.twilio.com) if you don't have one
     Purchase a sms-phone number.
     
 - Build & run the docker container: 
   ```
   #Build
   docker build -t pi .
   #Run (use --privileged to avoid dev/mem error when accessing RPi.GPIO hardware)
   docker run -d --privileged --restart always -p 5000:80 --name forpi pi
   ```

 - Start ngrok server:
   
   ```
   # naviate to ForPi/Pi/ngrok-stable-linux-arm folder
   ./ngrok authtoken <YourAuthenticationTokenFromNgrok>
   
   # start the server
   ./ngrok http 80 
   # if you run in Bad Gateway 502 then try the following:
   ./ngrok http 127.0.0.1:5000
   
   # This should show a unique http and https address which 
   # are forwarding request to your local aspnet core service
   # running on Pi
   
   # Add that to your twilio account:
   # - Log into Twilio.com and go to the Console's Numbers page.
   # - Click on your SMS-enabled phone number.
   # - Find the Messaging section. The default “CONFIGURE WITH” is what you’ll need: "Webhooks, TwiML, [etc.]".
   # - In the “A MESSAGE COMES IN” section, select "Webhook" and paste in your URL: in this quickstart step above, it would be: https://354f5b25.ngrok.io/sms - be sure to add /sms at the end, as this is the route to your SmsController class.
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
