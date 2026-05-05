import { ApolloClient, InMemoryCache, HttpLink, ApolloLink, concat } from '@apollo/client'

const createApolloClient = (token: string | null, uri: string) => {
  const httpLink = new HttpLink({
    uri
  })

  const authLink = new ApolloLink((operation, forward) => {
    if (token) {
      operation.setContext({
        headers: {
          authorization: `Bearer ${token}`
        }
      })
    }
    return forward(operation)
  })

  return new ApolloClient({
    ssrMode: typeof window === 'undefined',
    link: concat(authLink, httpLink),
    cache: new InMemoryCache()
  })
}

export default createApolloClient
