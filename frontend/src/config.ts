export const getApiUrl = (): string => {
  const stored = sessionStorage.getItem('GRAPHQL_ENDPOINT')
  if (stored) return stored

  if (import.meta.env.VITE_GRAPHQL_ENDPOINT) {
    return import.meta.env.VITE_GRAPHQL_ENDPOINT
  }

  return 'http://localhost:4000/graphql'
}

export const setApiUrl = (url: string): void => {
  sessionStorage.setItem('GRAPHQL_ENDPOINT', url)
}
