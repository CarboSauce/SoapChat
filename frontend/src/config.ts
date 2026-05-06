export const getApiUrl = (): string => {
  const stored = localStorage.getItem('GRAPHQL_ENDPOINT')
  if (stored) return stored

  if (import.meta.env.VITE_GRAPHQL_ENDPOINT) {
    return import.meta.env.VITE_GRAPHQL_ENDPOINT
  }

  return '/graphql'
}

export const setApiUrl = (url: string): void => {
  localStorage.setItem('GRAPHQL_ENDPOINT', url)
}
