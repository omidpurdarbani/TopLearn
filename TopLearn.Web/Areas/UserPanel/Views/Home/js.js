var file = document.getElementById("File");
var checkbox = document.getElementById("CheckBox");

checkbox.addEventListener("click" , ()=>{
    if(checkbox.checked==true){
        file.disabled=true;
    }
    if(checkbox.checked==false){
        file.disabled=false;
    }
})