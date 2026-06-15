export async function getClienti() {
    const raspuns = await fetch('http://localhost:5116/Client/GetClienti')
    if (!raspuns.ok) {
        throw new Error('Eroare la incarcarea clientilor')
    }
    return raspuns.json()
}

export async function saveClient(client) {
    return fetch('http://localhost:5116/Client/PostClientModel', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(client)
    })
}

export async function updateClient(client) {
    return fetch(`http://localhost:5116/Client/PutClientModel?id=${client.idClient}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(client)
    })
}

export async function deleteClient(id) {
    return fetch(`http://localhost:5116/Client/DeleteClientModel?id=${id}`, {
        method: 'DELETE'
    })
}
