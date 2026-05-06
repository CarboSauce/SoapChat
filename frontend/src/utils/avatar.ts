export const getAvatarUrl = (userId: string, apiUrl: string): string => {
  const baseUrl = apiUrl.replace('/graphql', '')
  return `${baseUrl}/api/avatar/${encodeURIComponent(userId)}`
}

export const extractBaseUrl = (graphqlUrl: string): string => {
  return graphqlUrl.replace('/graphql', '')
}

export const uploadAvatar = async (file: File, token: string, apiUrl: string): Promise<void> => {
  const baseUrl = extractBaseUrl(apiUrl)
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch(`${baseUrl}/api/avatar`, {
    method: 'POST',
    headers: {
      authorization: `Bearer ${token}`
    },
    body: formData
  })

  if (!response.ok) {
    throw new Error(`Upload failed: ${response.statusText}`)
  }
}

