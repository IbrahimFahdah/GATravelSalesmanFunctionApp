
const userAction = async () => {

    document.getElementsByClassName("app-progress-indicator")[0].style.visibility = "visible";

    var myBody = {
        NumCities: parseInt(document.getElementById("numofCities").value),
        PopulationSize: parseInt(document.getElementById("populationSize").value),
        CrossoverPercentage: parseInt(document.getElementById("crossoverPercentage").value),
        MutationPercentage: parseInt(document.getElementById("mutationPercentage").value),
    };

    console.log(myBody);

    const response = await fetch('https://gatravelsalesmanfunctionapp.azurewebsites.net/api/optimisedpath', {
        method: 'POST',
        body: JSON.stringify(myBody), 
    });

    const myJson = await response.json(); 

    console.log(myJson);

    document.getElementById("base64Img").src ='data:image/png;base64,'+ myJson.image;
    document.getElementById("bestLength").innerHTML  = myJson.bestLength;
    document.getElementById("nextNeighbourLength").innerHTML = myJson.nextNeighbourLength;
    let diff = parseInt(myJson.nextNeighbourLength) - parseInt(myJson.bestLength);
    document.getElementById("diffLength").innerHTML = diff;
    document.getElementById("success").innerHTML =  diff > 0 ? "&#x1F600;" : "&#x1F61E;";
    document.getElementsByClassName("app-progress-indicator")[0].style.visibility = "hidden";
}

function setBase64ToImage (){
     userAction();
}
