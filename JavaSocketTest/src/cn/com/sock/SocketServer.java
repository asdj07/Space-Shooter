package cn.com.sock;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.lang.reflect.Constructor;
import java.lang.reflect.Method;
import java.net.BindException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;

//import Server.ClientThread;


public class SocketServer {
    
    private final  String sp1 = ":";
    private final  String sp2 = "-";
    
    private ServerSocket serverSocket;
    private Socket client; 
    private BufferedReader in;
    private PrintWriter out;
    static int port = 8090;
    //static int max= 100;
    private ServerThread serverThread;  
    private ArrayList<ClientThread> clients; 
    private int max = 100;    //最大连接数 
    private boolean isStart = false;  
    
    private HashMap<String, HashMap<String, String>> map;   //存储各客户端的信息
    
    
    private static SocketServer socketServer;
    
    /*
    public static void main(String[] args) {
        new SocketServer();
    }*/
    
    public static SocketServer GetInstance(){
        if(socketServer == null){
            return new SocketServer();
        }else{
            return socketServer;
        }
    }
    
    
    private SocketServer() {  
    }
        
        
    // 启动服务器  
    public void serverStart() throws java.net.BindException {  
            try {  
                clients = new ArrayList<ClientThread>();     //客户端列表 
                serverSocket = new ServerSocket(port);       //端口，服务器
                serverThread = new ServerThread(serverSocket, max);    //服务器线程
                
                map = new HashMap<String, HashMap<String,String>>();
                
                serverThread.start();  
                isStart = true;  
                System.out.println("Server Start");
                
            } catch (BindException e) {  
                isStart = false;  
                throw new BindException("端口号:"+ port +"被占用");  
            } catch (Exception e1) {  
                e1.printStackTrace();  
                isStart = false;  
                throw new BindException("启动服务器异常！");  
            }  
    }  
      
    // 服务器线程     
    class ServerThread extends Thread {  
        private ServerSocket serverSocket;  
        private int max;// 人数上限  
  
        // 服务器线程的构造方法  
        public ServerThread(ServerSocket serverSocket, int max) {  
            this.serverSocket = serverSocket;  
            this.max = max;  
        }  
  
        public void run() {  
            while (true) {// 不停的等待客户端的链接  
                try {  
                    Socket socket = serverSocket.accept();  
                    System.out.println("Client : " + socket.getInetAddress() + " connect to server");
                    if (clients.size() == max) {// 如果已达人数上限  
                        BufferedReader r = new BufferedReader(  
                                new InputStreamReader(socket.getInputStream()));  
                        PrintWriter w = new PrintWriter(socket  
                                .getOutputStream());  

                        w.println("客户端连接受限，最大连接数：" + max + "!"); 
                        w.flush();  
                        // 释放资源  
                        r.close();  
                        w.close();  
                        socket.close();  
                        continue;  
                    }
                    ClientThread client = new ClientThread(socket);  
                    client.start();// 开启对此客户端服务的线程  
                    clients.add(client);  

                    //以客户端的ip作为key
                    map.put(socket.getInetAddress().toString(), new HashMap<String, String>());
                    
                } catch (IOException e) {  
                    e.printStackTrace();  
                }  
            }  
        }  
    }  
    
    
    
    class ClientThread extends Thread {  
        private Socket socket;  
        private BufferedReader reader;  
        private PrintWriter writer;  
        private String socket_ip;
  
        
        public String getSocket_ip() {
            return socket_ip;
        }
        

        // 客户端线程的构造方法  
        public ClientThread(Socket socket) {  
            try {  
                this.socket = socket;  
                reader = new BufferedReader(new InputStreamReader(socket  
                        .getInputStream()));  
                writer = new PrintWriter(socket.getOutputStream());  
                this.socket_ip = socket.getInetAddress().toString();   //客户端ip
            } catch (IOException e) {  
                e.printStackTrace();  
            }  
        }  
        
        public void run() {// 不断接收客户端的消息，进行处理。  
            String message = null;       
            boolean thread_flag = true;  //线程是否 没被终止
            String cName = null;         //请求的业务逻辑类
            String[] params = null;      //请求参数
            String return_msg = null;      //返回给客户端的消息
            ClientThread curThread = null;
            while (thread_flag) {  
                try {  
                    message = reader.readLine();// 接收客户端消息  
                    
                    if(socket.isClosed())//判断Socket是否关闭
                    {
                        thread_flag=false;//如果关闭,就跳出无限循环(while)
                        curThread.interrupt();
                        continue;
                    }
                    
                    System.out.println("receive msg : " + message);
                    
                    if (message == null)// 下线命令  
                    {  
                        reader.close();  
                        writer.close();  
                        socket.close();  
                        
                        // 删除此条客户端服务线程  
                        for (int i = clients.size() - 1; i >= 0; i--) {  
                            if (clients.get(i).getSocket_ip() == socket_ip) {  
                                curThread = clients.get(i);  
                                clients.remove(i);// 删除此用户的服务线程  
                                break;  
                            }  
                        }  
                        System.out.println("Client : " + socket_ip + " closed");
                        thread_flag = false;
                        curThread.interrupt();
                        continue;
                    }  
                    
                    cName = null;         
                    params = null;     
                    return_msg = null; 
                    
                    if(message.split(sp1)[0]!=null&&!("").equals(message.split(sp1)[0])){
                        cName = message.split(sp1)[0];
                        if(message.split(sp1).length>1 && (message.split(sp1)[1]!=null || !("").equals(message.split(sp1)[1]))){
                            String strParams = message.split(sp1)[1];
                            params = strParams.split(sp2);
                            return_msg = LoadMethod("cn.com.action."+cName,"execute",params);
                        }else{
                            return_msg = LoadMethod("cn.com.action."+cName,"execute");
                        }
                    }
                    
                     writer.println(return_msg);   //返回给客户端消息
                     writer.flush();  
                      
                } catch (IOException e) {  
                    e.printStackTrace();  
                    thread_flag = false;
                    curThread.interrupt();
                } 
            }  
        }  
        
        
        public String LoadMethod(String cName,String MethodName){
            String response_msg = null;
            Class cls;
            try {
                cls = Class.forName(cName);
                Constructor ct = cls.getConstructor(null); 
                Object obj = ct.newInstance(null);
                
                Method meth = cls.getMethod(MethodName); 
                response_msg= (String) meth.invoke(obj); 
                
            } catch (Throwable e) { 
                System.err.println(e); 
            } 
            return response_msg;
        }
        
        //根据类名 方法名调用类方法
        public String LoadMethod(String cName,String MethodName,String[] param){
            //Object retobj = null; 
            String response_msg = null;
            try{
                //加载指定的Java类
                Class cls = Class.forName(cName);
                
                //获取指定对象的实例 
                Constructor ct = cls.getConstructor(null); 
                Object obj = ct.newInstance(null); 
                
               //  构建方法参数的数据类型  都为String 
                Class[] cs = new Class[param.length]; 
                for (int i = 0; i < cs.length; i++) { 
                    cs[i]=String.class;
                }
                Class partypes[] = cs;
                
                
                String[] type = new String[param.length];
                for(int i = 0; i < type.length; i++){
                    type[i] = "String";
                }
                
                //构建方法参数的数据类型 
                //Class partypes[] = this.getMethodClass(type);
                
                //在指定类中获取指定的方法 
                Method meth = cls.getMethod(MethodName, partypes); 
                
                //构建方法的参数值 
                Object arglist[] = this.getMethodObject(type,param); 

                //调用指定的方法并获取返回值为Object类型 
                response_msg= (String) meth.invoke(obj, arglist); 
                
            }catch (Throwable e) { 
                System.err.println(e); 
            } 
            return response_msg;
        }
        
        
        public String LoadMethod(String cName,String MethodName,String[] type,String[] param){
            Object retobj = null; 
            
            try{
                //加载指定的Java类
                Class cls = Class.forName(cName);
                
                //获取指定对象的实例 
                Constructor ct = cls.getConstructor(null); 
                Object obj = ct.newInstance(null); 
                
                //构建方法参数的数据类型 
                Class partypes[] = this.getMethodClass(type);
                
              //在指定类中获取指定的方法 
                Method meth = cls.getMethod(MethodName, partypes); 
                
                //构建方法的参数值 
                Object arglist[] = this.getMethodObject(type,param); 

                //调用指定的方法并获取返回值为Object类型 
                retobj= meth.invoke(obj, arglist); 
                
            }catch (Throwable e) { 
                System.err.println(e); 
            } 
            
            
            return null;
        }
        
      //获取参数类型Class[]的方法 
        public Class[] getMethodClass(String[] type){ 
            Class[] cs = new Class[type.length]; 
            for (int i = 0; i < cs.length; i++) { 
                if(!type[i].trim().equals("")||type[i]!=null){ 
                    if(type[i].equals("int")||type[i].equals("Integer")){ 
                        cs[i]=Integer.TYPE; 
                    }else if(type[i].equals("float")||type[i].equals("Float")){ 
                        cs[i]=Float.TYPE; 
                    }else if(type[i].equals("double")||type[i].equals("Double")){ 
                        cs[i]=Double.TYPE; 
                    }else if(type[i].equals("boolean")||type[i].equals("Boolean")){ 
                        cs[i]=Boolean.TYPE; 
                    }else{ 
                        cs[i]=String.class; 
                    } 
                } 
            } 
            return cs; 
        } 

        //获取参数Object[]的方法 
        public Object[] getMethodObject(String[] type,String[] param){ 
            Object[] obj = new Object[param.length]; 
            for (int i = 0; i < obj.length; i++) { 
                if(!param[i].trim().equals("")||param[i]!=null){ 
                    if(type[i].equals("int")||type[i].equals("Integer")){ 
                        obj[i]= new Integer(param[i]); 
                    }else if(type[i].equals("float")||type[i].equals("Float")){ 
                        obj[i]= new Float(param[i]); 
                    }else if(type[i].equals("double")||type[i].equals("Double")){ 
                        obj[i]= new Double(param[i]); 
                    }else if(type[i].equals("boolean")||type[i].equals("Boolean")){ 
                        obj[i]=new Boolean(param[i]); 
                    }else{ 
                        obj[i] = param[i]; 
                    } 
                } 
            } 
            return obj; 
        } 
        
  
    } 

}
