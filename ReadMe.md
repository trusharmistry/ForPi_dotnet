## Raspberry Pi 4

 - Install Raspbian using NOOBs OS. Note: takes a while after 100% completion.
 - Enable VCN [youtube](https://www.youtube.com/watch?v=YP3_gvHZhfw) or [link](https://howtoraspberrypi.com/raspberry-pi-vnc/) for your Pi
   ```
   # run the command 
   sudo raspi-config
   # select the line Interfacing Options, then line VNC, 
   # and finally answer that you want to enable VNC
   ```
 - Connect all the components see [diagram](https://tutorials-raspberrypi.de/wp-content/uploads/2014/05/ultraschall_Steckplatine.png)
 - Install Docker on the Pi see instructions [here](https://linuxize.com/post/how-to-install-and-use-docker-on-raspberry-pi/)
 - Pull this git repo
 - Update application configuration file with credentials
   - Twilio account 
   - ngrok account
   
 - Build & run the docker container: 
   ```
   #Build
   docker build -t pi .
   #Run (use --privileged to avoid dev/mem error when accessing RPi.GPIO hardware)
   docker run -d --privileged -p 5000:80 --name forpi pi
   ```
 - Start ngrok server:
   ```
   
   ```
   
   
- Note if need to stop/delete old containers:
   ```
   docker container stop $(docker container ls -aq);
   docker container rm $(docker container ls -aq)
   ```
   
- (optional) Run Seq server:
  ```
  docker run -d --name seq-server --restart always -e "ACCEPT_EULA=Y" -p 5341:80 datalust/seq:latest;
  ```

- (optional) Run Seq server:)
