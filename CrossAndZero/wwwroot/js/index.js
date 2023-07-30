// https://localhost:7182/api/Account/Login
// https://localhost:7182/api/Account/Register
const btnSignIn = document.querySelector("#btnSignIn");
const btnRegister = document.querySelector("#btnRegister");
const authForm = document.querySelector("#authForm");
const userNameInp = document.querySelector("#username");
const passwordInp = document.querySelector("#password");
const gameInp = document.querySelector("#Game");
const btnFindGame = document.querySelector("#btnFindGame");

let token = localStorage.getItem("auth_key");
btnSignIn.addEventListener('click', Authenticate)
btnFindGame.addEventListener('click', FindGame)
async function Authenticate() {
    await fetch('https://localhost:7182/api/Game/Login', {
        method: 'post',
        headers: { 'Content-Type': 'application/json'},
        body: JSON.stringify({ 'userName': userNameInp.value, 'password': passwordInp.value}),
    })
    .then((response) => response.json())
    .then((data) => {
        console.log(data.token); 
        localStorage.setItem("auth_key",data.token); 
    }).catch(() => { console.log("exep"); localStorage.removeItem("auth_key");});

}
btnRegister.addEventListener('click', Register)
async function Register() {
    await fetch('https://localhost:7182/api/Game/Register', {
        method: 'post',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 'userName': userNameInp.value, 'password': passwordInp.value }),
    })
        .then((response) => response.json())
        .then((data) => {
            console.log(data.token);
            localStorage.setItem("auth_key", data.token);
        }).catch(() => { console.log("exep"); localStorage.removeItem("auth_key"); });
}
async function FindGame() {
    let headers = new Headers();
    headers.append('Authorization', 'Basic ' + token);
    let idGame = gameInp.value;
    await fetch('https://localhost:7182/api/Users/Game/'+idGame, {
        method: 'GET',
        headers: headers
    })
        .then((response) => response.json())
        .then((data) => {
            console.log(data);
        });
}
