import { useState, useRef } from 'react'
import { useApiUrl } from '../context/ApiContext'
import { useAuth } from '../context/AuthContext'
import { getAvatarUrl, uploadAvatar } from '../utils/avatar'
import '../styles/avatar.css'

interface AvatarProps {
  userId: string
  size?: 'small' | 'medium' | 'large'
  editable?: boolean
  onUploadComplete?: () => void
}

export default function Avatar({ userId, size = 'medium', editable = false, onUploadComplete }: AvatarProps) {
  const apiUrl = useApiUrl()
  const { token } = useAuth()
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [imageVersion, setImageVersion] = useState(0)

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

    if (!file.type.startsWith('image/')) {
      setError('Please select an image file')
      return
    }

    if (file.size > 5 * 1024 * 1024) {
      setError('Image must be smaller than 5MB')
      return
    }

    setError('')
    setIsLoading(true)

    try {
      if (!token) {
        throw new Error('Not authenticated')
      }
      await uploadAvatar(file, token, apiUrl)
      setImageVersion(v => v + 1)
      onUploadComplete?.()
    } catch (err) {
      setIsLoading(false)
      setError(err instanceof Error ? err.message : 'Upload failed')
    }

    if (fileInputRef.current) {
      fileInputRef.current.value = ''
    }
    setIsLoading(false)
  }

  const avatarUrl = `${getAvatarUrl(userId, apiUrl)}?v=${imageVersion}`
  const sizeClass = `avatar-${size}`

  return (
    <div className={`avatar-container ${sizeClass}`} title={editable ? 'Click edit button to change avatar' : ''}>
      <img
        src={avatarUrl}
        alt={`User avatar`}
        className="avatar-image"
        onError={(e) => {
          const img = e.target as HTMLImageElement
          img.src = getDefaultAvatar(userId)
        }}
      />
      {editable && (
        <button
          className="avatar-edit-button"
          onClick={() => fileInputRef.current?.click()}
          disabled={isLoading}
          title="Upload avatar"
          type="button"
        >
          {isLoading ? '...' : '✏️'}
        </button>
      )}
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        onChange={handleFileSelect}
        style={{ display: 'none' }}
        disabled={isLoading}
      />
      {error && <div className="avatar-error">{error}</div>}
    </div>
  )
}

const getDefaultAvatar = (userId: string): string => {
  const hash = userId.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  const hue = hash % 360
  return `data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='100' height='100'%3E%3Crect fill='hsl(${hue}, 70%25, 60%25)' width='100' height='100'/%3E%3Ctext x='50' y='50' text-anchor='middle' dy='.3em' fill='white' font-size='40' font-weight='bold'%3E${userId.charAt(0).toUpperCase()}%3C/text%3E%3C/svg%3E`
}
