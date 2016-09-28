package cn.com.action;

public class req001 {
    
    /*
     * 分数判别等级
     */
    public String execute(String score) {
        int score_value = Integer.valueOf(score);
        
        if(score_value >= 450){
            return "Penta Kill";
        }
        if(score_value >= 270){
            return "Quatary Kill";
        }
        if(score_value >= 150){
            return "Trible Kill";
        }
        if(score_value >= 60){
            return "Double Kill";
        }
        if(score_value >= 20){
            return "First Blood";
        }
       
        return "";
    }
    
    
    

}
