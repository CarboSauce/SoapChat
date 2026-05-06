import { ApolloProvider } from '@apollo/client'
import { useAuth, AuthProvider } from './context/AuthContext'
import { ApiProvider } from './context/ApiContext'
import { useMemo, useState, useEffect } from 'react'
import createApolloClient from './apollo'
import { getApiUrl } from './config'
import AuthPage from './pages/AuthPage'
import ChatPage from './pages/ChatPage'
import './App.css'

function AppContent() {
  const { token, loading } = useAuth()
  const [apiUrl, setApiUrl] = useState(getApiUrl())

  useEffect(() => {
    const url = getApiUrl()
    setApiUrl(url)
  }, [])

  const client = useMemo(() => createApolloClient(token, apiUrl), [token, apiUrl])

  if (loading) {
    return <div className="loading-container">Loading...</div>
  }

  return (
    <ApiProvider apiUrl={apiUrl}>
      <ApolloProvider client={client}>
        {token ? <ChatPage /> : <AuthPage onApiUrlChange={(newUrl) => setApiUrl(newUrl)} />}
      </ApolloProvider>
    </ApiProvider>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  )
}
