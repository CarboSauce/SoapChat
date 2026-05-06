import { createContext, useContext, ReactNode } from 'react'

interface ApiContextType {
  apiUrl: string
}

const ApiContext = createContext<ApiContextType | null>(null)

export const ApiProvider = ({ children, apiUrl }: { children: ReactNode; apiUrl: string }) => {
  return (
    <ApiContext.Provider value={{ apiUrl }}>
      {children}
    </ApiContext.Provider>
  )
}

export const useApiUrl = () => {
  const context = useContext(ApiContext)
  if (!context) {
    throw new Error('useApiUrl must be used within ApiProvider')
  }
  return context.apiUrl
}
