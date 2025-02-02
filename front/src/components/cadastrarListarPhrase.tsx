import { useState, useEffect } from "react";
import { Phrase } from "../models/Phrase";
import { useNavigate, useParams, Link } from "react-router-dom";
import axios from "axios";

function CadastroListarPhrase() {
    const navigate = useNavigate();
    const { Id } = useParams<{ Id: string }>();
    const [nativeLanguage, setNativeLanguage] = useState("");
    const [foreignLanguage, setForeignLanguage] = useState("");
    const [phrase, setPhrase] = useState<Phrase[]>([]);
    const [editId, setEditId] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string>("");

    useEffect(() => {
        carregarPhrase();
    }, []);

    function cadastrar(e: any) {
        e.preventDefault();
        setErrorMessage("");
        const newPhrase: Phrase = { nativeLanguage, foreignLanguage };
        fetch("http://localhost:5115/phrase/cadastrar", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newPhrase)
        }).then(async (resposta) => {
            if (!resposta.ok) {
                const errorText = await resposta.text();
                setErrorMessage(errorText);
                return;
            }
            return resposta.json();
        }).then(() => { carregarPhrase(); resetForm(); });
    }

    function alterar(e: any) {
        e.preventDefault();
        if (!editId) return;
        const fraseAtualizada: Phrase = { nativeLanguage, foreignLanguage };
        fetch(`http://localhost:5115/phrase/alterar/${editId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(fraseAtualizada)
        }).then((resposta) => resposta.json())
          .then(() => { carregarPhrase(); resetForm(); });
    }

    function carregarPhrase() {
        axios.get("http://localhost:5115/phrase/listar")
            .then(resposta => setPhrase(resposta.data))
            .catch((error) => { console.error('Erro ao listar:', error); setPhrase([]); });
    }

    function excluirPhrase(Id: number) {
        axios.delete(`http://localhost:5115/phrase/deletar/${Id}`)
            .then(() => carregarPhrase())
            .catch((error) => console.error('Erro ao excluir frase:', error));
    }

    function carregarEdicao(id: string, native: string, foreign: string) {
        setEditId(id);
        setNativeLanguage(native);
        setForeignLanguage(foreign);
    }

    function resetForm() {
        setEditId(null);
        setNativeLanguage("");
        setForeignLanguage("");
    }

    return (
        <div>
            {errorMessage && <p style={{ color: "red" }}>{errorMessage}</p>}
            <form onSubmit={editId ? alterar : cadastrar}>
                <fieldset>
                    <legend>{editId ? "Alterar Frase" : "Cadastrar Frase"}</legend>
                    <div>
                        <label>Em Português:</label>
                        <input type="text" value={nativeLanguage} onChange={(e) => setNativeLanguage(e.target.value)} required />
                    </div>
                    <div>
                        <label>Em Inglês:</label>
                        <input type="text" value={foreignLanguage} onChange={(e) => setForeignLanguage(e.target.value)} required />
                    </div>
                    <div>
                        <button type="submit">{editId ? "Salvar Alterações" : "Cadastrar"}</button>
                        {editId && <button type="button" onClick={resetForm}>Cancelar</button>}
                    </div>
                </fieldset>
            </form>
            <h2>Lista de Frases</h2>
            <table>
                <thead>
                    <tr>
                        <th>Em português</th>
                        <th>Em Inglês</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {phrase.length === 0 ? (
                        <tr>
                            <td colSpan={3}>Nenhuma frase encontrada.</td>
                        </tr>
                    ) : (
                        phrase.map(phrase => (
                            <tr key={phrase.id}>
                                <td>{phrase.nativeLanguage}</td>
                                <td>{phrase.foreignLanguage}</td>
                                <td>
                                    <button onClick={() => excluirPhrase(phrase.id!)}>Deletar</button>
                                    <button onClick={() => carregarEdicao(phrase.id!.toString(), phrase.nativeLanguage, phrase.foreignLanguage)}>Alterar</button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
}

export default CadastroListarPhrase;

