import { ApolloProvider } from '@apollo/client'
import { useAuth, AuthProvider } from './context/AuthContext'
import { useMemo, useState } from 'react'
import createApolloClient from './apollo'
import AuthPage from './pages/AuthPage'
import ChatPage from './pages/ChatPage'
import './App.css'

function AppContent() {
  const { token, loading } = useAuth()
  const [apiUrlVersion, setApiUrlVersion] = useState(0)

  const client = useMemo(() => createApolloClient(token), [token, apiUrlVersion])

  if (loading) {
    return <div className="loading-container">Loading...</div>
  }

  return (
    <ApolloProvider client={client}>
      {token ? <ChatPage /> : <AuthPage onApiUrlChange={() => setApiUrlVersion(v => v + 1)} />}
    </ApolloProvider>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  )
}
