import { useState } from 'react'
import { useMutation } from '@apollo/client'
import { LOGIN, REGISTER } from '../graphql'
import { useAuth } from '../context/AuthContext'
import { setApiUrl } from '../config'
import '../styles/auth.css'

interface AuthPageProps {
  onApiUrlChange: (newUrl: string) => void
}

export default function AuthPage({ onApiUrlChange }: AuthPageProps) {
  const [isLogin, setIsLogin] = useState(true)
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [apiUrl, setApiUrlInput] = useState('http://localhost:4000/graphql')
  const [error, setError] = useState('')
  const { login } = useAuth()

  const [loginMutation, { loading: loginLoading }] = useMutation(LOGIN, {
    onCompleted: (data) => {
      if (data.login.token) {
        login(data.login.user, data.login.token)
      }
    },
    onError: (err) => {
      setError(err.message)
    }
  })

  const [registerMutation, { loading: registerLoading }] = useMutation(REGISTER, {
    onCompleted: (data) => {
      if (data.register.token) {
        login(data.register.user, data.register.token)
      }
    },
    onError: (err) => {
      setError(err.message)
    }
  })

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    
    if (!username || !password) {
      setError('Username and password are required')
      return
    }

    setApiUrl(apiUrl)
    onApiUrlChange(apiUrl)

    setTimeout(() => {
      if (isLogin) {
        loginMutation({ variables: { username, password } })
      } else {
        registerMutation({ variables: { username, password } })
      }
    }, 0)
  }

  const loading = loginLoading || registerLoading

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1>{isLogin ? 'Login' : 'Register'}</h1>
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="api-url">API Endpoint:</label>
            <input
              id="api-url"
              type="text"
              value={apiUrl}
              onChange={(e) => setApiUrlInput(e.target.value)}
              placeholder="http://localhost:4000/graphql"
              className="form-input"
            />
          </div>

          <div className="form-group">
            <label htmlFor="username">Username:</label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Enter username"
              className="form-input"
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password:</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter password"
              className="form-input"
              disabled={loading}
            />
          </div>

          {error && <div className="error-message">{error}</div>}

          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Loading...' : (isLogin ? 'Login' : 'Register')}
          </button>
        </form>

        <p className="toggle-auth">
          {isLogin ? "Don't have an account? " : 'Already have an account? '}
          <button
            type="button"
            onClick={() => {
              setIsLogin(!isLogin)
              setError('')
            }}
            className="link-button"
          >
            {isLogin ? 'Register' : 'Login'}
          </button>
        </p>
      </div>
    </div>
  )
}
