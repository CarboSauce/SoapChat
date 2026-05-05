import { ApolloClient, InMemoryCache, HttpLink, ApolloLink, concat } from '@apollo/client'
import { getApiUrl } from './config'

const createApolloClient = (token: string | null) => {
  const httpLink = new HttpLink({
    uri: getApiUrl()
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
