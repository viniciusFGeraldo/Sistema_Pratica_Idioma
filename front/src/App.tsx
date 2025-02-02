import { BrowserRouter, Route, Routes } from "react-router-dom";
import { Link } from "react-router-dom";
import PaginaInicial from "./components/paginaInicial";
import CadastroListarPhrase from "./components/cadastrarListarPhrase";
import Parte from "./components/parte";

function App() {
  return (
    <BrowserRouter>
      <div>
        <nav>
          <ul>          
            <li>
              <Link to="/">Inicial</Link>
            </li>
            <li>
              <Link to="/cadastrarListar">Frases</Link>
            </li>
            <li>
              <Link to="/aula">aula</Link>
            </li>
          </ul>
        </nav>
        <Routes>
          <Route path="/" element={<PaginaInicial/>}></Route>
          <Route path="/cadastrarListar" element={<CadastroListarPhrase/>}></Route>
          <Route path="/aula" element={<Parte/>}></Route>
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
