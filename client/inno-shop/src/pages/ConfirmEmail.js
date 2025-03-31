import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { confirmEmail } from '../services/api';
import './ConfirmEmail.css'; 

function ConfirmEmail() {
  const { token } = useParams();
  const [message, setMessage] = useState("Подтверждение Email...");

  useEffect(() => {
    if (token) {
      console.log("Запрос отправляется с токеном:", token);
      confirmEmail(token)
        .then(() => {
          console.log("Запрос успешен");
          setMessage("Email successfully confirmed!");
        })
        .catch((error) => {
          console.log("Ошибка при подтверждении email", error);
          setMessage("Ошибка при подтверждении email.");
        });
    }
  }, [token]);

  return (
    <div className="confirm-email-page">
      <div className="confirm-email-message">
        {message}
      </div>
    </div>
  );
}

export default ConfirmEmail;
