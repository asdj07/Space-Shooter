package cn.com.sock;

import java.net.BindException;

public class Main {

    public static void main(String[] args) {
        // TODO Auto-generated method stub
        SocketServer socketserver = SocketServer.GetInstance();
        try {
            socketserver.serverStart();
        } catch (BindException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        
    }

}
